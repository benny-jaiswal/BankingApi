using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Model.Core.Model.Dto
{
    public class FinancialSummaryDto
    {
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal NetBalance { get; set; }
    }
}
