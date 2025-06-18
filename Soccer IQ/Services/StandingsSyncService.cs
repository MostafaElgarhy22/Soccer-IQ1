// Services/StandingsSyncService.cs
using System.Text.Json;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;

public class StandingsSyncService
{
    private readonly IRepository<LeagueStanding> _standingRepo;
    private readonly IRepository<Club> _clubRepo;
    private readonly HttpClient _http;

    private const string Token = "77ab2943ced945ae91b4c07bb1b5974e";          // 👈 ضع التوكن هنا
    private const string Endpoint = "https://api.football-data.org/v4/competitions/PL/standings";

    public StandingsSyncService(IRepository<LeagueStanding> standingRepo,
                                IRepository<Club> clubRepo,
                                IHttpClientFactory factory)
    {
        _standingRepo = standingRepo;
        _clubRepo = clubRepo;

        _http = factory.CreateClient();
        _http.DefaultRequestHeaders.Add("X-Auth-Token", Token);
    }

    public async Task<bool> SyncAsync()
    {
        var res = await _http.GetAsync(Endpoint);
        if (!res.IsSuccessStatusCode) return false;

        var json = await res.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(json).RootElement;

        // 🗓 موسم الحالى (year من startDate)
        var seasonYear = DateTime.Parse(
                            root.GetProperty("season")
                                .GetProperty("startDate")
                                .GetString()!).Year;

        // 📊 مصفوفة جدول الترتيب
        var table = root.GetProperty("standings")[0]
                        .GetProperty("table");

        foreach (var row in table.EnumerateArray())
        {
            var team = row.GetProperty("team");
            string name = team.GetProperty("name").GetString();
            string logo = team.GetProperty("crest").GetString();

            // ⚽ أوجد أو أنشئ النادى
            var club = _clubRepo.GetOne(null, c => c.Name == name, tracked: true);
            if (club == null)
            {
                club = new Club { Name = name, Country = "England", Logo = logo };
                _clubRepo.Create(club);
                _clubRepo.Commit();            // للحصول على Id فورًا
            }
            else if (club.Logo != logo)         // لو الشعار تغيّر
            {
                club.Logo = logo;
                _clubRepo.Edit(club);
            }

            // أرقام الجدول
            int pos = row.GetProperty("position").GetInt32();
            int played = row.GetProperty("playedGames").GetInt32();
            int wins = row.GetProperty("won").GetInt32();
            int draws = row.GetProperty("draw").GetInt32();
            int losses = row.GetProperty("lost").GetInt32();
            int gf = row.GetProperty("goalsFor").GetInt32();
            int ga = row.GetProperty("goalsAgainst").GetInt32();

            // 🔄 أضف أو حدّث السطر
            var standing = _standingRepo.GetOne(null,
                               s => s.ClubId == club.Id && s.Season == seasonYear,
                               tracked: true);

            if (standing == null)
            {
                standing = new LeagueStanding
                {
                    ClubId = club.Id,
                    Season = seasonYear,
                    Position = pos,
                    Played = played,
                    Wins = wins,
                    Draws = draws,
                    Losses = losses,
                    GoalsFor = gf,
                    GoalsAgainst = ga
                };
                _standingRepo.Create(standing);
            }
            else
            {
                standing.Position = pos;
                standing.Played = played;
                standing.Wins = wins;
                standing.Draws = draws;
                standing.Losses = losses;
                standing.GoalsFor = gf;
                standing.GoalsAgainst = ga;
                _standingRepo.Edit(standing);
            }
        }

        // حفظ التغييرات
        _clubRepo.Commit();
        _standingRepo.Commit();
        return true;
    }
}