using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Model.Core.Model.Dto
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public int ToAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
    }

}
