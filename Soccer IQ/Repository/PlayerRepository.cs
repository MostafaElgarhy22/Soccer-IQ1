using Soccer_IQ.Data;
using Soccer_IQ.Models;

public class PlayerRepository : BaseRepository<Player>
{
    public PlayerRepository(AppDbContext context) : base(context)
    {
    }
}
