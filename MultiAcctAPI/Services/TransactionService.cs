using MultiAcctAPI.Data;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDBContext _context;

        public TransactionService(AppDBContext context)
        {
            _context = context;
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == transaction.AccountId);
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

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction;
        }

        public IEnumerable<Transaction> GetTransactionsByAccountId(Guid accountId)
        {
            return _context.Transactions.Where(t => t.AccountId == accountId).ToList();
        }
    }
}