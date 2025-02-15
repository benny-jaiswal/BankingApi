using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterAPI.Core.Model;
using RegisterAPI.Infrastructure.Data.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Infrastructure.Data.Configuration
{
    public class RoleEntityModelMap : IEntityTypeConfiguration<RoleEnityModel>
    {
        public void Configure(EntityTypeBuilder<RoleEnityModel> builder) {

            builder.ToTable("Roles")
                .HasKey(u => u.RoleId);
            builder.Property(u => u.RoleName)
                .IsRequired()
                .HasMaxLength(128);                
        
        }
    }
}
