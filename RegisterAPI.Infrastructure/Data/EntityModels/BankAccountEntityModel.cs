using RegisterAPI.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Infrastructure.Data.EntityModels
{
    public class BankAccountEntityModel
    {
        public int AccountId { get; set; }
        public int UserId { get; set; } // Foreign key to Users table
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property (One User → Many Accounts)
        public UserEntityModel User { get; set; }

        // Navigation Property (One Account → Many Transactions)
        public ICollection<TransactionEntityModel> Transactions { get; set; } = new List<TransactionEntityModel>();

    }
}
