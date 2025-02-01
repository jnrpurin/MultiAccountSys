using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MultiAcctAPI.Data;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _context;
        private readonly string _secret;

        public UserService(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _secret = configuration.GetValue<string>("JwtSecret");
        }

        public User Register(User user)
        {
            if (_context.Users.Any(x => x.Email == user.Email))
                throw new InvalidOperationException("A user with this email already exists.");

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (user == null)
                return null;

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Ptt8Ri2GTXeuq1BmM4RHS0StgQ4QofEAVsyrHaPOHMA="); // Encoding.ASCII.GetBytes("3yJ2bG9yZSBpcCBzdWNoIGFuIGF3ZXNvbWUgc2VjcmV0IGtleQ==");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}