using MultiAcctAPI.Models;

namespace MultiAcctAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Transaction AddTransaction(Transaction transaction);
        IEnumerable<Transaction> GetTransactionsByAccountId(Guid accountId);
    }
}