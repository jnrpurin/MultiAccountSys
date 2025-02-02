using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiAcctAPI.Models;
using MultiAcctAPI.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Register a new user", Description = "Register a new user with the provided information.")]
        [SwaggerResponse(201, "User registered successfully", typeof(User))]
        [SwaggerResponse(409, "A user with this email already exists")]
        [SwaggerResponse(500, "An error occurred while registering the user")]
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
        [SwaggerOperation(Summary = "Authenticate a user", Description = "Authenticate a user with the provided email and password.")]
        [SwaggerResponse(200, "User authenticated successfully", typeof(User))]
        [SwaggerResponse(401, "Email or password is incorrect")]
        [SwaggerResponse(500, "An error occurred while authenticating the user")]
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
        [SwaggerOperation(Summary = "Get all users", Description = "Retrieve a list of all users.")]
        [SwaggerResponse(200, "List of users retrieved successfully", typeof(IEnumerable<User>))]
        [SwaggerResponse(500, "An error occurred while retrieving the users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user.", details = ex.Message });
            }  
        }
    }
}