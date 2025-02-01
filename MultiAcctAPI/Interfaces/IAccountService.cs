using MultiAcctAPI.Models;

namespace MultiAcctAPI.Services.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccountsByUserId(Guid userId);
        Account GetAccountById(Guid accountId);
        Account CreateAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(Guid accountId);
    }
}