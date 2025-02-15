using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterAPI.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Infrastructure.Data.Configuration
{
    public class UserEntityModelMap : IEntityTypeConfiguration<UserEntityModel>
    {
        public void Configure(EntityTypeBuilder<UserEntityModel> builder)
        {
            // Table Name
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(u => u.Id);

            // Properties
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(200);

            builder.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(256);

            builder.Property(u => u.DateOfBirth)
                    .IsRequired();

            builder.HasMany(u => u.UserRoles)
                   .WithOne(ur => ur.User)
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One User → Many BankAccounts
            builder.HasMany(u => u.BankAccounts)
                   .WithOne(ba => ba.User)
                   .HasForeignKey(ba => ba.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }       
    }
}


