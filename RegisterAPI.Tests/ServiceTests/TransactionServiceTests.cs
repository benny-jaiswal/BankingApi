using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RegisterAPI.Application.Services;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Data.EntityModels;
using RegisterAPI.Model.Core.Model.Dto;

namespace RegisterAPI.Tests.ServiceTests
{
    public class TransactionServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly Mock<ILogger<TransactionService>> _loggerMock;
        private readonly IMapper _mapper;

        public TransactionServiceTests()
        {
            // Setup in-memory database options (unique per test class run)
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // Setup ILogger mock
            _loggerMock = new Mock<ILogger<TransactionService>>();

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactionEntityModel, TransactionDto>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task DepositAsync_ValidAccount_ShouldIncreaseBalanceAndReturnTransactionDto()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var account = new BankAccountEntityModel
            {
                AccountId = 1,
                AccountNumber = "BA100001",
                Balance = 1000
            };
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new TransactionService(context, _loggerMock.Object, _mapper);

            // Act
            var result = await service.DepositAsync(accountId: 1, amount: 500);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Deposit", result.TransactionType);
            Assert.Equal(500, result.Amount);

            // Verify the updated balance in the database
            var updatedAccount = await context.BankAccounts.FindAsync(1);
            Assert.Equal(1500, updatedAccount.Balance);
        }

        [Fact]
        public async Task DepositAsync_NonExistentAccount_ShouldThrowException()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new TransactionService(context, _loggerMock.Object, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.DepositAsync(accountId: 999, amount: 100);
            });
        }

        [Fact]
        public async Task WithdrawAsync_ValidAccount_ShouldDecreaseBalanceAndReturnTransactionDto()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var account = new BankAccountEntityModel
            {
                AccountId = 2,
                AccountNumber = "BA100001",
                Balance = 800
            };
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new TransactionService(context, _loggerMock.Object, _mapper);

            // Act
            var result = await service.WithdrawAsync(accountId: 2, amount: 200);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Withdrawal", result.TransactionType);
            Assert.Equal(200, result.Amount);

            var updatedAccount = await context.BankAccounts.FindAsync(2);
            Assert.Equal(600, updatedAccount.Balance);
        }

        [Fact]
        public async Task WithdrawAsync_InsufficientBalance_ShouldThrowException()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var account = new BankAccountEntityModel
            {
                AccountId = 3,
                AccountNumber = "BA200001",
                Balance = 100
            };
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new TransactionService(context, _loggerMock.Object, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.WithdrawAsync(accountId: 3, amount: 200);
            });
        }

        [Fact]
        public async Task TransferAsync_ValidAccounts_ShouldTransferAmountAndReturnTransactionDto()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var fromAccount = new BankAccountEntityModel
            {
                AccountId = 1,
                AccountNumber = "BA100001",
                Balance = 500
            };
            var toAccount = new BankAccountEntityModel
            {
                AccountId = 2,
                AccountNumber = "BA100002",
                Balance = 1000
            };
            context.BankAccounts.AddRange(fromAccount, toAccount);
            await context.SaveChangesAsync();

            var service = new TransactionService(context, _loggerMock.Object, _mapper);

            // Act
            var result = await service.TransferAsync(fromAccountId: 1, toAccountId: 2, amount: 300);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Transfer", result.TransactionType);
            Assert.Equal(300, result.Amount);
            Assert.Equal(2, result.ToAccountId);

            var updatedFrom = await context.BankAccounts.FindAsync(1);
            var updatedTo = await context.BankAccounts.FindAsync(2);

            Assert.Equal(200, updatedFrom.Balance);
            Assert.Equal(1300, updatedTo.Balance);
        }

        [Fact]
        public async Task TransferAsync_InsufficientBalance_ShouldThrowException()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var fromAccount = new BankAccountEntityModel
            {
                AccountId = 1,
                AccountNumber = "BA100001",
                Balance = 100
            };
            var toAccount = new BankAccountEntityModel
            {
                AccountId = 2,
                AccountNumber = "BA100002",
                Balance = 5000
            };
            context.BankAccounts.AddRange(fromAccount, toAccount);
            await context.SaveChangesAsync();

            var service = new TransactionService(context, _loggerMock.Object, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.TransferAsync(fromAccountId: 11, toAccountId: 21, amount: 300);
            });
        }
    }
}
