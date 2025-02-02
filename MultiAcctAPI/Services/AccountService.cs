using Microsoft.EntityFrameworkCore;
using MultiAcctAPI.Data;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Models;
using MultiAcctAPI.ModelsAuxiliary;

namespace MultiAcctAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDBContext _context;

        public AccountService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new Account
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    CreationDate = a.CreationDate,
                    CurrentBalance = a.CurrentBalance,
                    UserId = a.UserId
                })
                .ToListAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            }
            return account;
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            account.AccountId = Guid.NewGuid();
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            var existingAccount = await GetAccountByIdAsync(account.AccountId);
            if (existingAccount != null)
            {
                existingAccount.AccountName = account.AccountName;
                existingAccount.CurrentBalance = account.CurrentBalance;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            var account = await GetAccountByIdAsync(accountId);
            if (account != null)
            {
                if (account.CurrentBalance != 0)
                {
                    throw new InvalidOperationException("Account current balance should be 0 (zero).");
                }
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetAccountBalanceAsync(Guid accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            return account.CurrentBalance;
        }

        public async Task<IEnumerable<AccountSummary>> GetUserAccountSummariesAsync(Guid userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new AccountSummary
                {
                    AccountId = a.AccountId,
                    Balance = a.CurrentBalance
                })
                .ToListAsync();
        }
    }
}