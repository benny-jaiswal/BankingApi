using Microsoft.EntityFrameworkCore;
using RegisterAPI.Application.Services;
using RegisterAPI.Infrastructure.Data;

namespace RegisterAPI.Tests
{
    public class AccountServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public AccountServiceTests()
        {
            // Create unique in-memory database for each test run.
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldCreateAccountWithZeroBalance()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new AccountService(context);
            int userId = 1;

            // Act
            var account = await service.CreateAccountAsync(userId);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(userId, account.UserId);
            Assert.NotNull(account.AccountNumber);
            Assert.NotEmpty(account.AccountNumber);
            Assert.Equal(0, account.Balance);
            Assert.StartsWith("BA", account.AccountNumber);
        }

        [Fact]
        public async Task GetAccountByIdAsync_ShouldReturnCorrectAccount()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new AccountService(context);
            int userId = 2;
            var createdAccount = await service.CreateAccountAsync(userId);

            // Act
            var retrievedAccount = await service.GetAccountByIdAsync(createdAccount.AccountId);

            // Assert
            Assert.NotNull(retrievedAccount);
            Assert.Equal(createdAccount.AccountId, retrievedAccount.AccountId);
            Assert.Equal(userId, retrievedAccount.UserId);
        }

        [Fact]
        public async Task GetAccountsByUserAsync_ShouldReturnAllAccountsForUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new AccountService(context);
            int userId = 3;

            // Create three accounts for the same user.
            await service.CreateAccountAsync(userId);
            await service.CreateAccountAsync(userId);
            await service.CreateAccountAsync(userId);

            // Act
            var accounts = await service.GetAccountsByUserAsync(userId);

            // Assert
            Assert.NotNull(accounts);
            Assert.Equal(3, accounts.Count);
            Assert.All(accounts, a => Assert.Equal(userId, a.UserId));
        }
    }
}
