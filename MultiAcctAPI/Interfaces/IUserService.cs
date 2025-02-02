using MultiAcctAPI.Models;

namespace MultiAcctAPI.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(User user);
        Task<User> AuthenticateAsync(string email, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}