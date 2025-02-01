using MultiAcctAPI.Models;

namespace MultiAcctAPI.Services.Interfaces
{
    public interface IUserService
    {
        User Register(User user);
        User Authenticate(string email, string password);
    }
}