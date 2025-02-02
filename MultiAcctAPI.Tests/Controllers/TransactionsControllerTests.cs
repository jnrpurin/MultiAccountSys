using Microsoft.AspNetCore.Mvc;
using Moq;
using MultiAcctAPI.Controllers;
using MultiAcctAPI.Enums;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Models;

namespace MultiAcctAPI.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionsController _controller;

        public TransactionsControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionsController(_mockTransactionService.Object);
        }

        [Fact]
        public async Task AddTransaction_ReturnsCreatedAtActionResult_WhenTransactionIsValid()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = 100,
                Type = TransactionType.Deposit,
                TransactionDate = DateTime.UtcNow
            };

            _mockTransactionService.Setup(service => service.AddTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await _controller.AddTransaction(transaction);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Transaction>(actionResult.Value);
            Assert.Equal(transaction.TransactionId, returnValue.TransactionId);
        }

        [Fact]
        public async Task GetTransactionsByAccountId_ReturnsOkResult_WithListOfTransactions()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        TransactionId = Guid.NewGuid(),
                        AccountId = accountId,
                        Amount = 100,
                        Type = TransactionType.Deposit,
                        TransactionDate = DateTime.UtcNow
                    }
                };

            // _mockTransactionService.Setup(service => service.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync(transactions);
            _mockTransactionService.Setup(service => service.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync((IEnumerable<Transaction>)transactions);

            // Act
            var result = await _controller.GetTransactionsByAccountId(accountId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Transaction>>(actionResult.Value);
            Assert.Single(returnValue);
        }
    }
}