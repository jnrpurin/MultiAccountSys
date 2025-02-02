using MultiAcctAPI.Data;
using MultiAcctAPI.Models;
using MultiAcctAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using MultiAcctAPI.Enums;

namespace MultiAcctAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDBContext _context;

        public TransactionService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == transaction.AccountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            if (transaction.Type == TransactionType.Withdrawal && account.CurrentBalance < transaction.Amount)
                throw new InvalidOperationException("Withdrawal amount exceeds account balance.");

            if (transaction.Type == TransactionType.Withdrawal)
                account.CurrentBalance -= transaction.Amount;
            else if (transaction.Type == TransactionType.Deposit)
                account.CurrentBalance += transaction.Amount;

            transaction.TransactionId = Guid.NewGuid();
            transaction.TransactionDate = DateTime.UtcNow;

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId)
        {
            return await _context.Transactions.Where(t => t.AccountId == accountId).ToListAsync();
        }
    }
}