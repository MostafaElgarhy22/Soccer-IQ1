using Microsoft.AspNetCore.Mvc;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;

[ApiController]
[Route("api/[controller]")]
public class LeagueStandingsController : ControllerBase
{
    private readonly IRepository<LeagueStanding> standingsRepo;

    public LeagueStandingsController(IRepository<LeagueStanding> standingsRepo)
    {
        this.standingsRepo = standingsRepo;
    }

    [HttpGet]
    public IActionResult GetAllStandings()
    {
        var standings = standingsRepo.GetAll();
        return Ok(standings);
    }

    [HttpGet("{id}")]
    public IActionResult GetStanding(int id)
    {
        var standing = standingsRepo.GetOne(null, s => s.Id == id);
        if (standing == null) return NotFound();
        return Ok(standing);
    }

    [HttpPost]
    public IActionResult CreateStanding([FromBody] LeagueStanding standing)
    {
        standingsRepo.Create(standing);
        standingsRepo.Commit();
        return CreatedAtAction(nameof(GetStanding), new { id = standing.Id }, standing);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateStanding(int id, [FromBody] LeagueStanding updatedStanding)
    {
        var standing = standingsRepo.GetOne(null, s => s.Id == id, tracked: true);
        if (standing == null) return NotFound();

        standing.Played = updatedStanding.Played;
        standing.Wins = updatedStanding.Wins;
        standing.Draws = updatedStanding.Draws;
        standing.Losses = updatedStanding.Losses;
        standing.GoalsFor = updatedStanding.GoalsFor;
        standing.GoalsAgainst = updatedStanding.GoalsAgainst;
        standing.ClubId = updatedStanding.ClubId;

        standingsRepo.Edit(standing);
        standingsRepo.Commit();

        return Ok(standing);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteStanding(int id)
    {
        var standing = standingsRepo.GetOne(null, s => s.Id == id, tracked: true);
        if (standing == null) return NotFound();

        standingsRepo.Delete(standing);
        standingsRepo.Commit();

        return NoContent();
    }
}
