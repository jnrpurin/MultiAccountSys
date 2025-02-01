using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

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
        public ActionResult<IEnumerable<Account>> GetAccountsByUserId(Guid userId)
        {
            try
            {
                var accounts = _accountService.GetAccountsByUserId(userId);
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
        public ActionResult<Account> GetAccountById(Guid accountId)
        {
            try
            {
                var account = _accountService.GetAccountById(accountId);
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
        public ActionResult<Account> CreateAccount(Account account)
        {
            try
            {
                var newAccount = _accountService.CreateAccount(account);
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
        public IActionResult UpdateAccount(Guid accountId, Account account)
        {
            if (accountId != account.AccountId)
                return BadRequest(new { message = "Account ID mismatch." });

            try
            {
                _accountService.UpdateAccount(account);
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
        public IActionResult DeleteAccount(Guid accountId)
        {
            try
            {
                _accountService.DeleteAccount(accountId);
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
    }
}