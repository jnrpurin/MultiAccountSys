using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.ModelsAuxiliary;
using Swashbuckle.AspNetCore.Annotations;

namespace MultiAcctAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        /// <summary>
        /// Get an account by user ID
        /// </summary>
        /// <param name="userId">User identification code</param>
        /// <returns>A list  of Accounts by the user id</returns>
        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Get accounts by user ID", Description = "Retrieve a list of accounts for a specific user.")]
        [SwaggerResponse(200, "List of accounts retrieved successfully", typeof(IEnumerable<Account>))]
        [SwaggerResponse(404, "No accounts found for the given user ID")]
        [SwaggerResponse(500, "An error occurred while retrieving accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountsByUserId(Guid userId)
        {
            try
            {
                var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                if (accounts == null)
                    return NotFound(new { message = "No accounts found for the given user ID." });

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving accounts.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get an account by account ID
        /// </summary>
        /// <param name="accountId">Account identification guid</param>
        /// <returns>The found account</returns>
        [HttpGet("{accountId}")]
        [SwaggerOperation(Summary = "Get account by ID", Description = "Retrieve the details of a specific account.")]
        [SwaggerResponse(200, "Account retrieved successfully", typeof(Account))]
        [SwaggerResponse(404, "Account not found")]
        [SwaggerResponse(500, "An error occurred while retrieving the account")]
        public async Task<ActionResult<Account>> GetAccountById(Guid accountId)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(accountId);
                if (account == null)
                    return NotFound(new { message = "Account not found." });

                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error ocurred while retrieving the account.", details = ex.Message });
            }
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        /// <param name="account">The account information</param>
        /// <returns>The account created</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new account", Description = "Create a new account for a user.")]
        [SwaggerResponse(201, "Account created successfully", typeof(Account))]
        [SwaggerResponse(500, "An error occurred while creating the account")]
        public async Task<ActionResult<Account>> CreateAccount(Account account)
        {
            try
            {
                var newAccount = await _accountService.CreateAccountAsync(account);
                return CreatedAtAction(nameof(GetAccountById), new { accountId = newAccount.AccountId }, newAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error ocurred while creating the account.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an account
        /// </summary>
        /// <param name="accountId">Account identification guid</param>
        /// <param name="account">The account information</param>
        /// <returns>Message action response for success or fail</returns>
        [HttpPut("{accountId}")]
        [SwaggerOperation(Summary = "Update an existing account", Description = "Update the details of an existing account.")]
        [SwaggerResponse(200, "Account updated successfully")]
        [SwaggerResponse(400, "Account ID mismatch")]
        [SwaggerResponse(500, "An error occurred while updating the account")]
        public async Task<IActionResult> UpdateAccount(Guid accountId, Account account)
        {
            if (accountId != account.AccountId)
                return BadRequest(new { message = "Account ID mismatch." });

            try
            {
                await _accountService.UpdateAccountAsync(account);
                return Ok(new {Success = "Account updated successfully."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the account.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="accountId">Account identification guid</param>
        /// <returns>Message action response for success or fail</returns>
        [HttpDelete("{accountId}")]
        [SwaggerOperation(Summary = "Delete an account", Description = "Delete an existing account.")]
        [SwaggerResponse(204, "Account deleted successfully")]
        [SwaggerResponse(500, "An error occurred while deleting the account")]
        public async Task<IActionResult> DeleteAccount(Guid accountId)
        {
            try
            {
                await _accountService.DeleteAccountAsync(accountId);
                return Ok(new {Success = "Account deleted successfully."});
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the account.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get the account balance
        /// </summary>
        /// <param name="accountId">Account identification guid</param>
        /// <returns>The current balance of a specific account</returns>
        [HttpGet("{accountId}/balance")]
        [SwaggerOperation(Summary = "Get account balance by account ID", Description = "Retrieve the current balance of a specific account.")]
        [SwaggerResponse(200, "Account balance retrieved successfully", typeof(decimal))]
        [SwaggerResponse(404, "Account not found")]
        [SwaggerResponse(500, "An error occurred while retrieving the account balance")]
        public async Task<ActionResult<decimal>> GetAccountBalance(Guid accountId)
        {
            try
            {
                var balance = await _accountService.GetAccountBalanceAsync(accountId);
                return Ok(balance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the account balance.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get account summaries by user ID
        /// </summary>
        /// <param name="userId">User identification code</param>
        /// <returns>The summary of all accounts and their balances for a user</returns>
        [HttpGet("user/{userId}/summaries")]
        [SwaggerOperation(Summary = "Get account summaries by user ID", Description = "Retrieve a summary of all accounts and their balances for a specific user.")]
        [SwaggerResponse(200, "Account summaries retrieved successfully", typeof(IEnumerable<AccountSummary>))]
        [SwaggerResponse(500, "An error occurred while retrieving account summaries")]
        public async Task<ActionResult<IEnumerable<AccountSummary>>> GetUserAccountSummaries(Guid userId)
        {
            try
            {
                var summaries = await _accountService.GetUserAccountSummariesAsync(userId);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving account summaries.", details = ex.Message });
            }
        }
    }
}