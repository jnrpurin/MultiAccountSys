using MultiAcctAPI.Data;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _context;

        public UserService(AppDBContext context)
        {
            _context = context;
        }

        public User Register(User user)
        {
            user.UserId = Guid.NewGuid();
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User Authenticate(string email, string password)
        {
            return _context.Users.SingleOrDefault(x => x.Email == email && x.Password == password);
        }
    }
}