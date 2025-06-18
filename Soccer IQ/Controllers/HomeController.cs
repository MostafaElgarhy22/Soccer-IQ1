using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;
using System.Linq.Expressions;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IRepository<LeagueStanding> standingsRepo;
    private readonly IRepository<Player> playerRepo;
    private readonly IRepository<PLayerStat> playerStatRepo;

    public HomeController(
        IRepository<LeagueStanding> standingsRepo,
        IRepository<Player> playerRepo,
        IRepository<PLayerStat> playerStatRepo)
    {
        this.standingsRepo = standingsRepo;
        this.playerRepo = playerRepo;
        this.playerStatRepo = playerStatRepo;
    }

    [HttpGet]
    public IActionResult GetHomeData()
    {
        // 🏆 جلب جدول الترتيب
        var standings = standingsRepo.GetAll(
            new Expression<Func<LeagueStanding, object>>[] { ls => ls.Club },
            null,
            tracked: false
        )
        .OrderBy(ls => ls.Position)
        .Select(ls => new
        {
            ls.Position,
            TeamName = ls.Club.Name,
            Logo = ls.Club.Logo,
            Played = ls.Played,
            Wins = ls.Wins,
            Draws = ls.Draws,
            Losses = ls.Losses,
            GoalsFor = ls.GoalsFor,
            GoalsAgainst = ls.GoalsAgainst,
            GoalDifference = ls.GoalDifference,
            Points = ls.Points
        })
        .ToList();

        // ⚽ جلب توب 3 هدافين
        var topScorers = playerStatRepo.GetAll(
            new Expression<Func<PLayerStat, object>>[] { ps => ps.Player, ps => ps.Player.Club },
            null,
            tracked: false
        )
        .OrderByDescending(ps => ps.Goals)
        .Take(3)
        .Select(ps => new
        {
            PlayerName = ps.Player.Name,
            Goals = ps.Goals,
            Club = ps.Player.Club.Name,
            Photo = ps.Player.PhotoUrl
        })
        .ToList();

        // 🅰️ جلب توب 3 أسيست
        var topAssists = playerStatRepo.GetAll(
            new Expression<Func<PLayerStat, object>>[] { ps => ps.Player, ps => ps.Player.Club },
            null,
            tracked: false
        )
        .OrderByDescending(ps => ps.Asissts)
        .Take(3)
        .Select(ps => new
        {
            PlayerName = ps.Player.Name,
            Assists = ps.Asissts,
            Club = ps.Player.Club.Name,
            Photo = ps.Player.PhotoUrl
        })
        .ToList();

        // 📦 بناء الرد النهائي
        var homeData = new
        {
            Standings = standings,
            TopScorers = topScorers,
            TopAssists = topAssists
        };

        return Ok(homeData);
    }
}
