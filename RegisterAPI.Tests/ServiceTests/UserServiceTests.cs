using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using RegisterAPI.Application.Services;
using RegisterAPI.Core.Model;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Data.Dto;

namespace RegisterAPI.Tests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly IConfiguration _config;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public UserServiceTests()
        {
            // Setup in-memory database options (each test gets a new database)
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Setup AutoMapper configuration.
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserEntityModel, UserDto>()
                   .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                   .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                   .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            });
            _mapper = mappingConfig.CreateMapper();

            // Setup IConfiguration using in-memory collection.
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "SuperSecretKey123456789012345678"},
                {"Jwt:Issuer", "http://localhost"},
                {"Jwt:Audience", "http://localhost"}
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Setup IHttpContextAccessor mock.
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext());
        }

        [Fact]
        public async Task Authenticate_InvalidCredentials_ReturnsNull()
        {
            // Arrange: Use an empty in-memory database.
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new UserService(context, _config, _httpContextAccessorMock.Object, _mapper);

            // Act
            var result = await service.Authenticate("nonexistent", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsUserDtoWithToken()
        {
            // Arrange: Create a test user.
            using var context = new ApplicationDbContext(_dbOptions);
            var testUser = new UserEntityModel
            {
                Id = 1,
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "password123" // For testing (no hashing here)
            };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var service = new UserService(context, _config, _httpContextAccessorMock.Object, _mapper);

            // Act: Authenticate with valid credentials.
            var result = await service.Authenticate("testuser", "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }

        [Fact]
        public async Task RegisterUserAsync_ValidRegistration_ReturnsTrue()
        {
            // Arrange: Create a new in-memory database.
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new UserService(context, _config, _httpContextAccessorMock.Object, _mapper);

            var registrationDto = new UserRegistrationDto
            {
                UserName = "JohnDoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            };

            // Act
            var result = await service.RegisterUserAsync(registrationDto);

            // Assert
            Assert.True(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }
    }
}
