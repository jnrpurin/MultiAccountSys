using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Account>> GetAccountsByUserId(Guid userId)
        {
            var accounts = _accountService.GetAccountsByUserId(userId);
            return Ok(accounts);
        }

        [HttpGet("{accountId}")]
        public ActionResult<Account> GetAccountById(Guid accountId)
        {
            var account = _accountService.GetAccountById(accountId);
            if (account == null)
                return NotFound();

            return Ok(account);
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount(Account account)
        {
            var newAccount = _accountService.CreateAccount(account);
            return CreatedAtAction(nameof(GetAccountById), new { accountId = newAccount.AccountId }, newAccount);
        }

        [HttpPut("{accountId}")]
        public IActionResult UpdateAccount(Guid accountId, Account account)
        {
            if (accountId != account.AccountId)
                return BadRequest();

            _accountService.UpdateAccount(account);
            return NoContent();
        }

        [HttpDelete("{accountId}")]
        public IActionResult DeleteAccount(Guid accountId)
        {
            _accountService.DeleteAccount(accountId);
            return NoContent();
        }
    }
}