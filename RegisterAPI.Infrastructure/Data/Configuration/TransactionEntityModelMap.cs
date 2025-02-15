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
    public class TransactionEntityMap : IEntityTypeConfiguration<TransactionEntityModel>
    {
        public void Configure(EntityTypeBuilder<TransactionEntityModel> builder)
        {
            // Table Name
            builder.ToTable("Transactions");

            // Primary Key
            builder.HasKey(t => t.TransactionId);

            // Properties
            builder.Property(t => t.TransactionId)
                   .ValueGeneratedOnAdd(); // Auto-increment

            builder.Property(t => t.TransactionType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.TransactionDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Enforce allowed values for TransactionType
            builder.HasCheckConstraint("CHK_TransactionType", "TransactionType IN ('Deposit', 'Withdrawal', 'Transfer')");

            // Relationships
            builder.HasOne(t => t.Account) // Foreign key to BankAccounts
                   .WithMany(ba => ba.Transactions)
                   .HasForeignKey(t => t.AccountId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete transactions if account is deleted

            // Self-referencing relationship for transfers
            builder.HasOne(t => t.ToAccount)
                   .WithMany()
                   .HasForeignKey(t => t.ToAccountId)
                   .OnDelete(DeleteBehavior.Restrict); // Restrict deletion if referenced in a transfer
        }
    }
}
