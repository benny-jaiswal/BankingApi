using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RegisterAPI.Application.Services;
using RegisterAPI.Controllers;
using RegisterAPI.Model.Core.Model.Dto;

namespace RegisterAPI.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<ILogger<TransactionController>> _loggerMock;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _loggerMock = new Mock<ILogger<TransactionController>>();
            _controller = new TransactionController(_transactionServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Deposit_ReturnsOkResult_WithTransactionDto()
        {
            // Arrange
            var request = new TransactionRequestModel { AccountId = 1, Amount = 100 };
            var transactionDto = new TransactionDto
            {
                TransactionId = 1,
                AccountId = 1,
                Amount = 100,
                TransactionType = "Deposit",
                TransactionDate = DateTime.UtcNow
            };

            _transactionServiceMock
                .Setup(s => s.DepositAsync(request.AccountId, request.Amount))
                .ReturnsAsync(transactionDto);

            // Act
            var result = await _controller.Deposit(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transactionDto, okResult.Value);
        }

        [Fact]
        public async Task Withdraw_ReturnsOkResult_WithTransactionDto()
        {
            // Arrange
            var request = new TransactionRequestModel { AccountId = 2, Amount = 50 };
            var transactionDto = new TransactionDto
            {
                TransactionId = 2,
                AccountId = 2,
                Amount = 50,
                TransactionType = "Withdrawal",
                TransactionDate = DateTime.UtcNow
            };

            _transactionServiceMock
                .Setup(s => s.WithdrawAsync(request.AccountId, request.Amount))
                .ReturnsAsync(transactionDto);

            // Act
            var result = await _controller.Withdraw(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transactionDto, okResult.Value);
        }

        [Fact]
        public async Task Transfer_ReturnsOkResult_WithTransactionDto()
        {
            // Arrange
            var request = new TransferRequestModel { FromAccountId = 1, ToAccountId = 2, Amount = 75 };
            var transactionDto = new TransactionDto
            {
                TransactionId = 3,
                AccountId = 1,
                Amount = 75,
                TransactionType = "Transfer",
                TransactionDate = DateTime.UtcNow
            };

            _transactionServiceMock
                .Setup(s => s.TransferAsync(request.FromAccountId, request.ToAccountId, request.Amount))
                .ReturnsAsync(transactionDto);

            // Act
            var result = await _controller.Transfer(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transactionDto, okResult.Value);
        }

        [Fact]
        public async Task GetRecentTransactions_ReturnsOkResult_WithTransactionsList()
        {
            // Arrange
            int accountId = 1;
            var transactions = new List<TransactionDto>
            {
                new TransactionDto { TransactionId = 1, AccountId = accountId, Amount = 100, TransactionType = "Deposit", TransactionDate = DateTime.UtcNow },
                new TransactionDto { TransactionId = 2, AccountId = accountId, Amount = 50, TransactionType = "Withdrawal", TransactionDate = DateTime.UtcNow }
            };

            _transactionServiceMock
                .Setup(s => s.GetRecentTransactionsAsync(accountId, 5))
                .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetRecentTransactions(accountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transactions, okResult.Value);
        }

        [Fact]
        public async Task GetFinancialSummary_ReturnsOkResult_WithFinancialSummaryDto()
        {
            // Arrange
            int accountId = 1;
            var summaryDto = new FinancialSummaryDto
            {
                TotalDeposits = 500,
                TotalWithdrawals = 200,
                NetBalance = 300
            };

            _transactionServiceMock
                .Setup(s => s.GetFinancialSummaryAsync(accountId))
                .ReturnsAsync(summaryDto);

            // Act
            var result = await _controller.GetFinancialSummary(accountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(summaryDto, okResult.Value);
        }
    }
}
