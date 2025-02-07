using Microsoft.EntityFrameworkCore;
using RegisterAPI.Core.Model;

namespace RegisterAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEntityModel> Users { get; set; } 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);


            base.OnModelCreating(modelBuilder);
        }
    }
}
