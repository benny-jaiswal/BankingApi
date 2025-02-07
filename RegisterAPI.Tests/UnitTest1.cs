using Microsoft.AspNetCore.Mvc;
using Moq;
using RegisterAPI.Controllers;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Infrastructure.Services;


namespace RegisterAPI.tests
{
    public class RegisterAPITests
    {
        private readonly Mock<IUserService> _mockUserService;
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
        public RegisterAPITests()
        {
            _mockUserService = new Mock<IUserService>();
            _userController = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async void GetUsers_ReturnOkResult_WhenUserFound()
        {
            _mockUserService.Setup(Service => Service.GetAllUsersAsync()).ReturnsAsync(userList);

            var result = await  _userController.GetUsers();
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);  // Check if the result is OkObjectResult
            Assert.NotNull(okResult);
            Assert.Equal(userList, okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

        }

        //[Fact]
        //public void GetFibonacci_ValidCount_ReturnsCorrectSequence()
        //{
        //    var controler = new UserController();
        //    Assert.Pass();
        //}
    }
}