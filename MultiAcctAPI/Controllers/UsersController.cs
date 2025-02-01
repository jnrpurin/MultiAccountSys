using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace MultiAcctAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns>Message action response for success or fail</returns>
        [HttpPost("registration")]
        [AllowAnonymous]
        // [SwaggerRequestExample(typeof(User), typeof(UserRegistrationExample))]
        public IActionResult Register(User user)
        {
            try
            {
                var newUser = _userService.Register(user);
                return CreatedAtAction(nameof(Register), new { id = newUser.UserId }, newUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user.", details = ex.Message });
            }
        }

        /// <summary>
        /// Authenticate a user
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns>Message action response for success or fail</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] User user)
        {
            var authenticatedUser = _userService.Authenticate(user.Email, user.Password);
            if (authenticatedUser == null)
                return Unauthorized(new { message = "Email or password is incorrect" });

            return Ok(new { Token = authenticatedUser.Token });
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Users from data base</returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
    }
}