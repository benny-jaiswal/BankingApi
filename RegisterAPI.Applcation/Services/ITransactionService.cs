using RegisterAPI.Model.Core.Model.Dto;

namespace RegisterAPI.Application.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto> DepositAsync(int accountId, decimal amount);
        Task<TransactionDto> WithdrawAsync(int accountId, decimal amount);
        Task<TransactionDto> TransferAsync(int fromAccountId, int toAccountId, decimal amount);
        Task<List<TransactionDto>> GetRecentTransactionsAsync(int accountId, int count);
        Task<FinancialSummaryDto> GetFinancialSummaryAsync(int accountId);
    }
}
