using Microsoft.EntityFrameworkCore;
using Moq;
using MultiAcctAPI.Data;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services;

namespace MultiAcctAPI.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<AppDBContext> _mockContext;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            _mockContext = new Mock<AppDBContext>(new DbContextOptions<AppDBContext>());
            _accountService = new AccountService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldAddAccount()
        {
            // Arrange
            var account = new Account { AccountName = "Account1", UserId = Guid.NewGuid(), CurrentBalance = 100, CreationDate = DateTime.Now };

            var mockSet = new Mock<DbSet<Account>>();
            _mockContext.Setup(c => c.Accounts).Returns(mockSet.Object);

            // Act
            var result = await _accountService.CreateAccountAsync(account);

            // Assert
            mockSet.Verify(m => m.AddAsync(It.IsAny<Account>(), default), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
            Assert.NotEqual(Guid.Empty, result.AccountId);
        }
    }
}