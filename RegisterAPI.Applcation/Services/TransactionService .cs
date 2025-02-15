using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Data.EntityModels;
using RegisterAPI.Model.Core.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterAPI.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionService> _logger;
        private readonly IMapper _mapper;

        public TransactionService(ApplicationDbContext context, ILogger<TransactionService> logger, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TransactionDto> DepositAsync(int accountId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"Starting deposit transaction. AccountId: {accountId}, Amount: {amount}");

                var account = await _context.BankAccounts.FindAsync(accountId);
                if (account == null)
                {
                    _logger.LogWarning($"Deposit failed: Account {accountId} not found.");
                    throw new Exception("Account not found.");
                }

                account.Balance += amount;

                var transaction = new TransactionEntityModel
                {
                    AccountId = accountId,
                    TransactionType = "Deposit",
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deposit successful. AccountId: {accountId}, New Balance: {account.Balance}");

                return _mapper.Map<TransactionDto>(transaction); 

            }
            catch (Exception Ex)
            {
                _logger.LogInformation($"Error in DepositAsync: {Ex.Message}");
                throw;
            }
        }


        public async Task<TransactionDto> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"Starting transfer transaction. FromAccountId: {fromAccountId}, ToAccountId: {toAccountId}, Amount: {amount}");

                var fromAccount = await _context.BankAccounts.FindAsync(fromAccountId);
                var toAccount = await _context.BankAccounts.FindAsync(toAccountId);

                if (fromAccount == null || toAccount == null)
                {
                    _logger.LogWarning($"Transfer failed: One or both accounts not found. FromAccountId: {fromAccountId}, ToAccountId: {toAccountId}");
                    throw new Exception("One or both accounts not found.");
                }

                if (fromAccount.Balance < amount)
                {
                    _logger.LogWarning($"Transfer failed: Insufficient balance in FromAccountId: {fromAccountId}");
                    throw new Exception("Insufficient balance.");
                }

                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                var transaction = new TransactionEntityModel
                {
                    AccountId = fromAccountId,
                    TransactionType = "Transfer",
                    Amount = amount,
                    ToAccountId = toAccountId,
                    TransactionDate = DateTime.UtcNow
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Transfer successful. FromAccountId: {fromAccountId}, ToAccountId: {toAccountId}, New Balance (From): {fromAccount.Balance}, New Balance (To): {toAccount.Balance}");

                return  _mapper.Map<TransactionDto>(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in TransferAsync: {ex.Message}");
                throw;
            }
        }

         public async Task<TransactionDto> WithdrawAsync(int accountId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"Starting withdrawal transaction. AccountId: {accountId}, Amount: {amount}");

                var account = await _context.BankAccounts.FindAsync(accountId);
                if (account == null)
                {
                    _logger.LogWarning($"Withdrawal failed: Account {accountId} not found.");
                    throw new Exception("Account not found.");
                }

                if (account.Balance < amount)
                {
                    _logger.LogWarning($"Withdrawal failed: Insufficient balance in AccountId: {accountId}");
                    throw new Exception("Insufficient balance.");
                }

                account.Balance -= amount;

                var transaction = new TransactionEntityModel
                {
                    AccountId = accountId,
                    TransactionType = "Withdrawal",
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Withdrawal successful. AccountId: {accountId}, New Balance: {account.Balance}");

                return _mapper.Map<TransactionDto>(transaction); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in WithdrawAsync: {ex.Message}");
                throw;
            }
        }
        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int accountId, int count)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    AccountId = t.AccountId,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();
        }

        // ✅ Generate Financial Summary
        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(int accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync();

            return new FinancialSummaryDto
            {
                TotalDeposits = transactions.Where(t => t.TransactionType == "Deposit").Sum(t => t.Amount),
                TotalWithdrawals = transactions.Where(t => t.TransactionType == "Withdraw").Sum(t => t.Amount),
                NetBalance = transactions.Sum(t => t.Amount)
            };
        }
    }
}
