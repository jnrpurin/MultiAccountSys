using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

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

        [HttpPost]
        public IActionResult AddTransaction(Transaction transaction)
        {
            try
            {
                var newTransaction = _transactionService.AddTransaction(transaction);
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

        [HttpGet("account/{accountId}")]
        public IActionResult GetTransactionsByAccountId(Guid accountId)
        {
            try
            {
                var transactions = _transactionService.GetTransactionsByAccountId(accountId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving transactions.", details = ex.Message });
            }
        }
    }
}