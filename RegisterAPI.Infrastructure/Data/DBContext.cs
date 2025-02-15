using Microsoft.EntityFrameworkCore;
using RegisterAPI.Core.Model;
using RegisterAPI.Infrastructure.Data.EntityModels;

namespace RegisterAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEntityModel> Users { get; set; }
        public DbSet<UserRoleEntityModel> UserRoles { get; set; }
        public DbSet<UserRoleEntityModel> Roles { get; set; }
        public DbSet<BankAccountEntityModel> BankAccounts { get; set; }
        public DbSet<TransactionEntityModel> Transactions { get; set; }
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
