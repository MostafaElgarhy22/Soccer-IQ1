using Microsoft.AspNetCore.Mvc;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;

[ApiController]
[Route("api/[controller]")]
public class PlayerStatsController : ControllerBase
{
    private readonly IRepository<PLayerStat> playerStatRepo;

    public PlayerStatsController(IRepository<PLayerStat> playerStatRepo)
    {
        this.playerStatRepo = playerStatRepo;
    }

    [HttpGet]
    public IActionResult GetAllPlayerStats()
    {
        var playerStats = playerStatRepo.GetAll();
        return Ok(playerStats);
    }

    [HttpGet("{id}")]
    public IActionResult GetPlayerStat(int id)
    {
        var playerStat = playerStatRepo.GetOne(null, ps => ps.Id == id);
        if (playerStat == null) return NotFound();
        return Ok(playerStat);
    }

    [HttpPost]
    public IActionResult CreatePlayerStat([FromBody] PLayerStat playerStat)
    {
        playerStatRepo.Create(playerStat);
        playerStatRepo.Commit();
        return CreatedAtAction(nameof(GetPlayerStat), new { id = playerStat.Id }, playerStat);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePlayerStat(int id, [FromBody] PLayerStat updatedPlayerStat)
    {
        var playerStat = playerStatRepo.GetOne(null, ps => ps.Id == id, tracked: true);
        if (playerStat == null) return NotFound();

        playerStat.Goals = updatedPlayerStat.Goals;
        playerStat.Asissts = updatedPlayerStat.Asissts;
        playerStat.MinutesPlayed = updatedPlayerStat.MinutesPlayed;
        playerStat.Matches = updatedPlayerStat.Matches;
        playerStat.Season = updatedPlayerStat.Season;
        playerStat.XG = updatedPlayerStat.XG;
        playerStat.XA = updatedPlayerStat.XA;
        playerStat.PlayerId = updatedPlayerStat.PlayerId;

        playerStatRepo.Edit(playerStat);
        playerStatRepo.Commit();

        return Ok(playerStat);
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePlayerStat(int id)
    {
        var playerStat = playerStatRepo.GetOne(null, ps => ps.Id == id, tracked: true);
        if (playerStat == null) return NotFound();

        playerStatRepo.Delete(playerStat);
        playerStatRepo.Commit();

        return NoContent();
    }
}
