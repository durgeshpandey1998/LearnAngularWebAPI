using LearnAngularWebAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace LearnAngularWebAPI.DataContext
{
    public class DataContext2: DbContext
    {
        public DataContext2(DbContextOptions<DataContext2> options):
            base(options) { }
        public virtual DbSet<UserModel> UserRegistration { get; set; }
    }
}
