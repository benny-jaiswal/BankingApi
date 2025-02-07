using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RegisterAPI.Core.Model;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Infrastructure.Services;
using System.Security.Claims;

namespace RegisterAPI.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _contextAccessor = httpContext;
        }
        
        public async Task<IEnumerable<UserDto>>GetAllUsersAsync() 
        {         
            return await _context.Users
                    .Select(x => new UserDto { FirstName = x.FirstName, LastName = x.LastName, Email = x.Email })
                    .ToListAsync();     
        }
        // note here fi yu change to sysn use Task.FromResuly other wise retrun dto
        public Task<UserDto> GetUser()
        {
            UserDto userDto = new UserDto()
            {
                FirstName = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value
            };
            return Task.FromResult(userDto);
        }

        public async Task<bool> RegisterUserAsync(UserRegistrationDto userRegistrationDto) {
            if (userRegistrationDto == null)
            {
                throw new ArgumentNullException(nameof(userRegistrationDto), "Registration data must not be null.");
            }

            if (string.IsNullOrWhiteSpace(userRegistrationDto.FirstName) ||
                string.IsNullOrWhiteSpace(userRegistrationDto.LastName) ||
                string.IsNullOrWhiteSpace(userRegistrationDto.Email) ||
                string.IsNullOrWhiteSpace(userRegistrationDto.Password))
            {
                throw new ArgumentException("All fields must be filled", nameof(userRegistrationDto));
            }
            var user = new UserEntityModel
            {
                FirstName = userRegistrationDto?.FirstName,
                LastName = userRegistrationDto?.LastName,
                Email = userRegistrationDto.Email,
                Password = userRegistrationDto.Password
            };

            _context.Users.Add(user);
            var success = await _context.SaveChangesAsync();
            return success > 0;       
                
        }
    }
}
