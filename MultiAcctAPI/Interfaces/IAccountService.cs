using MultiAcctAPI.Models;
using MultiAcctAPI.ModelsAuxiliary;

namespace MultiAcctAPI.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId);
        Task<Account> GetAccountByIdAsync(Guid accountId);
        Task<Account> CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Guid accountId);
        Task<decimal> GetAccountBalanceAsync(Guid accountId);
        Task<IEnumerable<AccountSummary>> GetUserAccountSummariesAsync(Guid userId);
    }
}