using Employee_Self_Service.Modals;
using Microsoft.EntityFrameworkCore;

namespace Employee_Self_Service.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<emp_info> emp_info => Set<emp_info>();
    }
}
