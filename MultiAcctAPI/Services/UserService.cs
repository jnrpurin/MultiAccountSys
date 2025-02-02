using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiAcctAPI.Data;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task<User> RegisterAsync(User user)
        {
            if (_context.Users.Any(x => x.Email == user.Email))
                throw new InvalidOperationException("A user with this email already exists.");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password) ?? throw new InvalidOperationException("Email or password is incorrect.");

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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}