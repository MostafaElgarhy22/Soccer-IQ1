using Soccer_IQ.Data;
using Soccer_IQ.Models;

public class ClubRepository : BaseRepository<Club>
{
    public ClubRepository(AppDbContext context) : base(context)
    {
    }
}
