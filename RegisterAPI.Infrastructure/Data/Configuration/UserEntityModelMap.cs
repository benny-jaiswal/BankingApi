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

            builder.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(256);

            builder.Property(u => u.DateOfBirth)
                    .IsRequired();

            // Additional constraints (if any) can be added here
        }       
    }
}


