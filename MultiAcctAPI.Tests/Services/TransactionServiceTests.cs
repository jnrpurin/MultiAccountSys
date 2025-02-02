using Microsoft.EntityFrameworkCore;
using MultiAcctAPI.Data;
using MultiAcctAPI.Enums;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services;

namespace MultiAcctAPI.Tests
{
    public class TransactionServiceTests
    {
        private readonly TransactionService _transactionService;
        private readonly AppDBContext _context;

        public TransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDBContext(options);
            _transactionService = new TransactionService(_context);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldAddTransaction()
        {
            // Arrange
            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                AccountName = "Test Account",
                CurrentBalance = 1000,
                CreationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = 100,
                Type = TransactionType.Deposit
            };

            // Act
            var result = await _transactionService.AddTransactionAsync(transaction);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.AccountId, result.AccountId);
            Assert.Equal(transaction.Amount, result.Amount);
            Assert.Equal(transaction.Type, result.Type);
            Assert.Equal(account.CurrentBalance, account.CurrentBalance);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldThrowException_WhenAccountNotFound()
        {
            // Arrange
            var transaction = new Transaction
            {
                AccountId = Guid.NewGuid(),
                Amount = 100,
                Type = TransactionType.Deposit
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.AddTransactionAsync(transaction));
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldThrowException_WhenWithdrawalExceedsBalance()
        {
            // Arrange
            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                AccountName = "Test Account",
                CurrentBalance = 100,
                CreationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = 200,
                Type = TransactionType.Withdrawal
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.AddTransactionAsync(transaction));
        }
    }
}
