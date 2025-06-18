using Microsoft.AspNetCore.Mvc;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;
using System.Linq.Expressions;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IRepository<Player> playerRepo;

    public PlayersController(IRepository<Player> playerRepo)
    {
        this.playerRepo = playerRepo;
    }

    // Get All Players
    [HttpGet]
    public IActionResult GetAllPlayers()
    {
        var players = playerRepo.GetAll(
            new Expression<Func<Player, object>>[] { p => p.Club },
            null,
            tracked: false
        );
        return Ok(players);
    }

    // Get Player by ID
    [HttpGet("{id}")]
    public IActionResult GetPlayer(int id)
    {
        var player = playerRepo.GetOne(
            new Expression<Func<Player, object>>[] { p => p.Club },
            p => p.Id == id,
            tracked: false
        );

        if (player == null) return NotFound();
        return Ok(player);
    }

    // Create Player
    [HttpPost]
    public IActionResult CreatePlayer([FromBody] Player player)
    {
        playerRepo.Create(player);
        playerRepo.Commit();
        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
    }

    // Update Player
    [HttpPut("{id}")]
    public IActionResult UpdatePlayer(int id, [FromBody] Player updatedPlayer)
    {
        var player = playerRepo.GetOne(null, p => p.Id == id, tracked: true);
        if (player == null) return NotFound();

        player.Name = updatedPlayer.Name;
        player.Height = updatedPlayer.Height;
        player.Age = updatedPlayer.Age;
        player.MainPosition = updatedPlayer.MainPosition;
        player.OtherPosition = updatedPlayer.OtherPosition;
        player.StrongFoot = updatedPlayer.StrongFoot;
        player.MarketValue = updatedPlayer.MarketValue;
        player.ClubId = updatedPlayer.ClubId;

        playerRepo.Edit(player);
        playerRepo.Commit();

        return Ok(player);
    }

    // Delete Player
    [HttpDelete("{id}")]
    public IActionResult DeletePlayer(int id)
    {
        var player = playerRepo.GetOne(null, p => p.Id == id, tracked: true);
        if (player == null) return NotFound();

        playerRepo.Delete(player);
        playerRepo.Commit();

        return NoContent();
    }
}
