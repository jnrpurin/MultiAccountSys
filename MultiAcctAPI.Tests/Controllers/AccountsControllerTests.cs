using Microsoft.AspNetCore.Mvc;
using Moq;
using MultiAcctAPI.Controllers;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Models;

namespace MultiAcctAPI.Tests.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountsController(_mockAccountService.Object);
        }

        [Fact]
        public async Task GetAccountsByUserId_ReturnsOkResult_WithListOfAccounts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accounts = new List<Account>
            {
                new Account { AccountId = Guid.NewGuid(), UserId = userId, AccountName = "Account1",  CurrentBalance = 100 },
                new Account { AccountId = Guid.NewGuid(), UserId = userId, AccountName = "Account2", CurrentBalance = 200 }
            };
            _mockAccountService.Setup(service => service.GetAccountsByUserIdAsync(userId)).ReturnsAsync(accounts);

            // Act
            var result = await _controller.GetAccountsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnAccounts = Assert.IsType<List<Account>>(okResult.Value);
            Assert.Equal(2, returnAccounts.Count);
        }

        [Fact]
        public async Task GetAccountById_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            _mockAccountService.Setup(service => service.GetAccountByIdAsync(accountId)).ReturnsAsync((Account?)null);

            // Act
            var result = await _controller.GetAccountById(accountId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}