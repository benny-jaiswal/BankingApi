using RegisterAPI.Infrastructure.Data.Dto;

namespace RegisterAPI.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> RegisterUserAsync(UserRegistrationDto userDto);
        Task<UserDto> GetUser();
        Task<UserDto> Authenticate(string username, string password);
        List<string> GetUserRoles(int userId);
    }

}
