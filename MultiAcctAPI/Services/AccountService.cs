using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly List<Account> _accounts = new List<Account>();

        public IEnumerable<Account> GetAccountsByUserId(Guid userId)
        {
            return _accounts.Where(a => a.UserId == userId).ToList();
        }

        public Account GetAccountById(Guid accountId)
        {
            return _accounts.SingleOrDefault(a => a.AccountId == accountId);
        }

        public Account CreateAccount(Account account)
        {
            account.AccountId = Guid.NewGuid();
            _accounts.Add(account);
            return account;
        }

        public void UpdateAccount(Account account)
        {
            var existingAccount = GetAccountById(account.AccountId);
            if (existingAccount != null)
            {
                existingAccount.AccountName = account.AccountName;
                existingAccount.CurrentBalance = account.CurrentBalance;
            }
        }

        public void DeleteAccount(Guid accountId)
        {
            var account = GetAccountById(accountId);
            if (account != null)
            {
                _accounts.Remove(account);
            }
        }
    }
}