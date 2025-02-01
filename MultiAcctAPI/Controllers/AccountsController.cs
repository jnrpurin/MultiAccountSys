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

        [HttpPut("{accountId}")]
        public IActionResult UpdateAccount(Guid accountId, Account account)
        {
            if (accountId != account.AccountId)
                return BadRequest(new { message = "Account ID mismatch." });

            try
            {
                _accountService.UpdateAccount(account);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the account.", details = ex.Message });
            }
        }

        [HttpDelete("{accountId}")]
        public IActionResult DeleteAccount(Guid accountId)
        {
            try
            {
                _accountService.DeleteAccount(accountId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the account.", details = ex.Message });
            }
        }
    }
}