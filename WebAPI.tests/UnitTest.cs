using RegisterAPI.Controllers;
using Xunit;
using Moq;
using RegisterAPI.Infrastructure.Services;
using RegisterAPI.Infrastructure.Data.Dto;


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
        public void GetUsers_ReturnOkResult_WhenUserFound()
        {
            _mockUserService.Setup(Service => Service.GetAllUsersAsync()).ReturnsAsync(userList);

            var result = _userController.GetUsers().Result ;
            
            Assert.Null(result);
            Assert.Equal(userList, (result.Value.ToList()));

        }

        //[Fact]
        //public void GetFibonacci_ValidCount_ReturnsCorrectSequence()
        //{
        //    var controler = new UserController();
        //    Assert.Pass();
        //}
    }
}