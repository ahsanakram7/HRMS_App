using Employee_Self_Service.Modals.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Employee_Self_Service.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //public DbSet<RegisterUser> registerUsers => Set<RegisterUser>();
        public DbSet<emp_info> emp_info => Set<emp_info>();
        public DbSet<Screens> screens => Set<Screens>();
        public DbSet<RoleActivities> roleActivities => Set<RoleActivities>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<emp_info>(entity =>
            {
                entity.HasKey(e => e.emp_no);

                entity.Property(e => e.emp_no)
                      .ValueGeneratedNever();
            });
        }
    }
}
