using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;

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
        public async Task<ActionResult<User>> Register(User user)
        {
            try
            {
                var newUser = await _userService.RegisterAsync(user);
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
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            try
            {
                var authenticatedUser = await _userService.AuthenticateAsync(user.Email, user.Password);
                return Ok(new { authenticatedUser.Token });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user.", details = ex.Message });
            }            
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Users from data base</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}