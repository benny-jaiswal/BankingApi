using Microsoft.AspNetCore.Mvc;
using Moq;
using RegisterAPI.Controllers;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Infrastructure.Services;

namespace RegisterAPI.Tests
{
    public class RegisterAPIUnitTest2
    {
        private readonly Mock<IUserService> _userService;
        private readonly UserController _userController;

        private static List<UserDto> userList = new List<UserDto>() {
                new UserDto()
                {
                    FirstName ="test" ,
                    LastName="user2",
                    Email ="test2@gmail.com"
                },
                new UserDto()
                {
                    FirstName ="test" ,
                    LastName="user1",
                    Email ="test@gmail.com"
                },
            };
        public RegisterAPIUnitTest2() {
            _userService = new Mock<IUserService>();
            _userController = new UserController(); 
        }


        [Fact]
        public async void GetUsers_ReturnOkResult_WhenuserFound()
        {
        _userService.Setup( service => service.GetAllUsersAsync()).ReturnsAsync(userList);

            var result = await _userController.GetUsers();
        ;
            var okResult=Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.NotNull(okResult)
            Assert.Equal(200, okResult.StatusCode)
            Assert.Equal(userList, okResult.Value);
        }
    }
}
