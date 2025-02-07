using RegisterAPI.Infrastructure.Data.Dto;

namespace RegisterAPI.Infrastructure.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> RegisterUserAsync(UserRegistrationDto userDto);
        Task<UserDto> GetUser();
    }

}
