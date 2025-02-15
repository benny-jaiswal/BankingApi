using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterAPI.Application.Services;
using RegisterAPI.Model.Core.Model.Dto;

namespace RegisterAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] TransactionRequestModel request)
        {
            var result = await _transactionService.DepositAsync(request.AccountId, request.Amount);
            return Ok(result);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequestModel request)
        {
            var result = await _transactionService.WithdrawAsync(request.AccountId, request.Amount);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestModel request)
        {
            var result = await _transactionService.TransferAsync(request.FromAccountId, request.ToAccountId, request.Amount);
            return Ok(result);
        }

        [HttpGet("recent/{accountId}")]
        public async Task<IActionResult> GetRecentTransactions(int accountId)
        {
            var transactions = await _transactionService.GetRecentTransactionsAsync(accountId, 5); // Last 5 transactions
            return Ok(transactions);
        }

        // New API: Financial Summary for Dashboard
        [HttpGet("summary/{accountId}")]
        public async Task<IActionResult> GetFinancialSummary(int accountId)
        {
            var summary = await _transactionService.GetFinancialSummaryAsync(accountId);
            return Ok(summary);
        }

    }
}
