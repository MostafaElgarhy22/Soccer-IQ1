using Soccer_IQ.Data;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;

public class ApplicationUserRepository : BaseRepository<ApplicationUser>, IRepository<ApplicationUser>
{
    public ApplicationUserRepository(AppDbContext okk ) : base(okk) { }
}

