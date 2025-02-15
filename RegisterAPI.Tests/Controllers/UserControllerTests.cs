using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RegisterAPI.Application.Services;
using RegisterAPI.Controllers;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Model.Core.Model.Dto;
using Xunit;

namespace RegisterAPI.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithUserList()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { UserName = "user1", FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new UserDto { UserName = "user2", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" }
            };
            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                            .ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(users, returnedUsers);
        }

        [Fact]
        public async Task Authorize_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            LoginDto login = null; // null login

            // Act
            var result = await _controller.Authorize(login);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid input", badRequest.Value);
        }

        [Fact]
        public async Task Authorize_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var login = new LoginDto { UserName = "testuser", PasswordHash = "wrongpass" };
            _userServiceMock.Setup(s => s.Authenticate(login.UserName, login.PasswordHash))
                            .ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.Authorize(login);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Authorize_ValidCredentials_ReturnsOkResult_WithUserDto()
        {
            // Arrange
            var login = new LoginDto { UserName = "testuser", PasswordHash = "password123" };
            var userDto = new UserDto
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Token = "generated_token"
            };
            _userServiceMock.Setup(s => s.Authenticate(login.UserName, login.PasswordHash))
                            .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Authorize(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userDto.UserName, returnedUser.UserName);
            Assert.Equal("generated_token", returnedUser.Token);
        }

        [Fact]
        public async Task RegisterUsers_SuccessfulRegistration_ReturnsCreatedAtAction()
        {
            // Arrange
            var registrationDto = new UserRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registrationDto))
                            .ReturnsAsync(true);

            // Act
            var result = await _controller.RegisterUsers(registrationDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetUsers), createdAtResult.ActionName);
        }

        [Fact]
        public async Task RegisterUsers_FailedRegistration_ReturnsBadRequest()
        {
            // Arrange
            var registrationDto = new UserRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registrationDto))
                            .ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterUsers(registrationDto);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
    }
}
