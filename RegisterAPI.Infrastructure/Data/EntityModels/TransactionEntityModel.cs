namespace RegisterAPI.Infrastructure.Data.EntityModels
{
    public class TransactionEntityModel
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; } // Foreign key to BankAccounts
        public string TransactionType { get; set; } // Deposit, Withdrawal, Transfer
        public decimal Amount { get; set; }
        public int? ToAccountId { get; set; } // Nullable (for transfers)
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public BankAccountEntityModel Account { get; set; }
        public BankAccountEntityModel ToAccount { get; set; } // Self-referencing for transfers
    }
}
