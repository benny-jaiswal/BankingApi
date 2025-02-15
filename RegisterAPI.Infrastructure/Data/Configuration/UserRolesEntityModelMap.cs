using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterAPI.Infrastructure.Data.EntityModels;


namespace RegisterAPI.Infrastructure.Data.Configuration
{
    public class UserRolesEntityModelMap : IEntityTypeConfiguration<UserRoleEntityModel>
    {

        public void Configure(EntityTypeBuilder<UserRoleEntityModel> builder)
        {

            builder.ToTable("UserRoles");
            builder.HasKey(e => e.UserRoleId);
            builder.Property(e => e.RoleId)
                .IsRequired();
            builder.Property(e => e.UserId)
                .IsRequired();
            builder.HasOne(ur => ur.User)
                   .WithMany(ur => ur.UserRoles)
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(ur => ur.Role)
           .WithMany()
           .HasForeignKey(ur => ur.RoleId)
           .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
