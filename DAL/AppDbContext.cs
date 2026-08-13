using Employee_Self_Service.Modals;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Employee_Self_Service.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<RegisterUser> registerUsers => Set<RegisterUser>();
        public DbSet<emp_info> emp_info => Set<emp_info>();
    }
}
