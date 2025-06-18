using Soccer_IQ.Data;
using Soccer_IQ.Models;

public class PlayerStatRepository : BaseRepository<PLayerStat>
{
    public PlayerStatRepository(AppDbContext context) : base(context)
    {
    }
}
