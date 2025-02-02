using MultiAcctAPI.Models;

namespace MultiAcctAPI.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId);
    }
}