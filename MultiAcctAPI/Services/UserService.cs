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
        private readonly IConfiguration _configuration;

        public UserService(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
            var user = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password) ?? throw new InvalidOperationException("Email or password is incorrect.");

            // Generate JWT token
            var jwtSettings = _configuration.GetSection("JWTSecrets");
            var jwtSecret = jwtSettings["JwtSecret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var issuer = jwtSettings["JwtIssuer"];
            var audience = jwtSettings["JwtAudience"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.Name, user.UserId.ToString()),
                    new (ClaimTypes.Email, user.Email),
                    new (ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = issuer,
                Audience = audience,
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