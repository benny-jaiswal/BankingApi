using Microsoft.EntityFrameworkCore;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Data.EntityModels;

namespace RegisterAPI.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BankAccountEntityModel> CreateAccountAsync(int userId)
        {
            var account = new BankAccountEntityModel
            {
                UserId = userId,
                AccountNumber = GenerateAccountNumber(),
                Balance = 0
            };

            _context.BankAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<BankAccountEntityModel> GetAccountByIdAsync(int accountId)
        {
            return await _context.BankAccounts.FindAsync(accountId);
        }

        public async Task<List<BankAccountEntityModel>> GetAccountsByUserAsync(int userId)
        {
            return await _context.BankAccounts
                                .Where(a => a.UserId == userId)
                                .ToListAsync();
        }

        private string GenerateAccountNumber()
        {
            return "BA" + new System.Random().Next(100000, 999999).ToString();
        }
    }
}
