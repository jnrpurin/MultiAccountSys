using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MultiAcctAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Add a new transaction
        /// </summary>
        /// <param name="transaction">Transaction information</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Add a new transaction", Description = "Add a new transaction (Deposit or Withdrawal) to an account.")]
        [SwaggerResponse(201, "Transaction added successfully", typeof(Transaction))]
        [SwaggerResponse(400, "Invalid transaction request")]
        [SwaggerResponse(500, "An error occurred while processing the transaction")]
        public async Task<ActionResult<Transaction>> AddTransaction(Transaction transaction)
        {
            try
            {
                var newTransaction = await _transactionService.AddTransactionAsync(transaction);
                return CreatedAtAction(nameof(AddTransaction), new { id = newTransaction.TransactionId }, newTransaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the transaction.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get transactions by account ID
        /// </summary>
        /// <param name="accountId">Account identification code</param>
        /// <returns>Transactions of the specific account</returns>
        [HttpGet("account/{accountId}")]
        [SwaggerOperation(Summary = "Get transactions by account ID", Description = "Retrieve a list of all transactions for a specific account.")]
        [SwaggerResponse(200, "List of transactions retrieved successfully", typeof(IEnumerable<Transaction>))]
        [SwaggerResponse(500, "An error occurred while retrieving transactions")]
        public async Task<IActionResult> GetTransactionsByAccountId(Guid accountId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving transactions.", details = ex.Message });
            }
        }
    }
}