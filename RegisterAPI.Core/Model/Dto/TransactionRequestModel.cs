using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Model.Core.Model.Dto
{
    public class TransactionRequestModel
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
