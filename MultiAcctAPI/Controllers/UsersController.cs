using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services;
using MultiAcctAPI.Services.Interfaces;

namespace MultiAcctAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            var newUser = _userService.Register(user);
            return Ok(newUser);
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User user)
        {
            var authenticatedUser = _userService.Authenticate(user.Email, user.Password);
            if (authenticatedUser == null)
                return Unauthorized();

            // Generate JWT token here 
            return Ok(authenticatedUser);
        }
    }
}