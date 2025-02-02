using Microsoft.AspNetCore.Mvc;
using Moq;
using MultiAcctAPI.Controllers;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAcctAPI.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task Register_ReturnsCreatedAtActionResult_WhenUserIsRegistered()
        {
            // Arrange
            var user = new User { Name = "Test User", Email = "test@example.com", Password = "password" };
            _mockUserService.Setup(service => service.RegisterAsync(It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var result = await _controller.Register(user);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(user.Email, returnValue.Email);
        }

        [Fact]
        public async Task Authenticate_ReturnsOkResult_WhenUserIsAuthenticated()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Password = "password", Name = "test" };
            var authenticatedUser = new User { Email = "test@example.com", Token = "token", Name = "auth", Password = "password" };
            _mockUserService.Setup(service => service.AuthenticateAsync(user.Email, user.Password)).ReturnsAsync(authenticatedUser);

            // Act
            var result = await _controller.Authenticate(user);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = actionResult.Value;

            var returnData = JObject.FromObject(returnValue);

            Assert.NotNull(returnData);
            Assert.Equal("token", returnData["Token"]?.ToString());
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User> { new User { Email = "test1@example.com", Name = "test", Password = "password" }, new User { Email = "test2@example.com", Name = "test", Password = "password" } };
            _mockUserService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync((IEnumerable<User>)users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<User>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}
