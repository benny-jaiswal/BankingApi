using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RegisterAPI.Infrastructure.Data.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Infrastructure.Data.Configuration
{
    public class BankAccountEntityMap : IEntityTypeConfiguration<BankAccountEntityModel>
    {
        public void Configure(EntityTypeBuilder<BankAccountEntityModel> builder)
        {
            // Table Name
            builder.ToTable("BankAccounts");

            // Primary Key
            builder.HasKey(ba => ba.AccountId);

            // Properties
            builder.Property(ba => ba.AccountId)
                   .ValueGeneratedOnAdd(); // Identity column

            builder.Property(ba => ba.AccountNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(ba => ba.Balance)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            builder.Property(ba => ba.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Unique Constraint for Account Number
            builder.HasIndex(ba => ba.AccountNumber)
                   .IsUnique();

            // Relationships
            builder.HasOne(ba => ba.User)
                   .WithMany(u => u.BankAccounts)
                   .HasForeignKey(ba => ba.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Cascade delete when User is deleted

            builder.HasMany(ba => ba.Transactions)
                   .WithOne(t => t.Account)
                   .HasForeignKey(t => t.AccountId)
                   .OnDelete(DeleteBehavior.Cascade); // Cascade delete when Account is deleted
        }
    }
}
