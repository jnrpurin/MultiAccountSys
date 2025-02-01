using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>();

        public User Register(User user)
        {
            user.UserId = Guid.NewGuid();
            _users.Add(user);
            return user;
        }

        public User Authenticate(string email, string password)
        {
            return _users.SingleOrDefault(x => x.Email == email && x.Password == password);
        }
    }
}