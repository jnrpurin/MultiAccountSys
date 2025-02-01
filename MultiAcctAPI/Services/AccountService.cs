using MultiAcctAPI.Data;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDBContext _context;

        public AccountService(AppDBContext context)
        {
            _context = context;
        }

        public IEnumerable<Account> GetAccountsByUserId(Guid userId)
        {
            return _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new Account
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    CreationDate = a.CreationDate,
                    CurrentBalance = a.CurrentBalance,
                    UserId = a.UserId
                })
                .ToList();
        }

        public Account GetAccountById(Guid accountId)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == accountId)!;
        }

        public Account CreateAccount(Account account)
        {
            account.AccountId = Guid.NewGuid();
            _context.Accounts.Add(account);
            _context.SaveChanges();
            return account;
        }

        public void UpdateAccount(Account account)
        {
            var existingAccount = GetAccountById(account.AccountId);
            if (existingAccount != null)
            {
                existingAccount.AccountName = account.AccountName;
                existingAccount.CurrentBalance = account.CurrentBalance;
                _context.SaveChanges();
            }
        }

        public void DeleteAccount(Guid accountId)
        {
            var account = GetAccountById(accountId);
            if (account != null)
            {
                if (account.CurrentBalance != 0)
                {
                    throw new InvalidOperationException("Account current balance should be 0 (zero).");
                }
                _context.Accounts.Remove(account);
                _context.SaveChanges();
            }
        }
    }
}