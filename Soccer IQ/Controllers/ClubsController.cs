using Microsoft.AspNetCore.Mvc;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;

[ApiController]
[Route("api/[controller]")]
public class ClubsController : ControllerBase
{
    private readonly IRepository<Club> clubRepo;

    public ClubsController(IRepository<Club> clubRepo)
    {
        this.clubRepo = clubRepo;
    }

    [HttpGet]
    public IActionResult GetAllClubs()
    {
        var clubs = clubRepo.GetAll();
        return Ok(clubs);
    }

    [HttpGet("{id}")]
    public IActionResult GetClub(int id)
    {
        var club = clubRepo.GetOne(null, c => c.Id == id);
        if (club == null) return NotFound();
        return Ok(club);
    }

    [HttpPost]
    public IActionResult CreateClub([FromBody] Club club)
    {
        clubRepo.Create(club);
        clubRepo.Commit();
        return CreatedAtAction(nameof(GetClub), new { id = club.Id }, club);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateClub(int id, [FromBody] Club updatedClub)
    {
        var club = clubRepo.GetOne(null, c => c.Id == id, tracked: true);
        if (club == null) return NotFound();

        club.Name = updatedClub.Name;
        club.Country = updatedClub.Country;
        club.Logo = updatedClub.Logo;

        clubRepo.Edit(club);
        clubRepo.Commit();

        return Ok(club);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteClub(int id)
    {
        var club = clubRepo.GetOne(null, c => c.Id == id, tracked: true);
        if (club == null) return NotFound();

        clubRepo.Delete(club);
        clubRepo.Commit();

        return NoContent();
    }
}
