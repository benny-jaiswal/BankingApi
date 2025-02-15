using RegisterAPI.Infrastructure.Data.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Application.Services
{
    public interface IAccountService
    {
        Task<BankAccountEntityModel> CreateAccountAsync(int userId);
        Task<BankAccountEntityModel> GetAccountByIdAsync(int accountId);
        Task<List<BankAccountEntityModel>> GetAccountsByUserAsync(int userId);
    }
}
