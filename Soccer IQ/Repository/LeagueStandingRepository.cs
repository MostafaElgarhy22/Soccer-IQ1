using Soccer_IQ.Data;
using Soccer_IQ.Models;

public class LeagueStandingRepository : BaseRepository<LeagueStanding>
{
    public LeagueStandingRepository(AppDbContext context) : base(context)
    {
    }
}
