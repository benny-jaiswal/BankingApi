using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Application.Services;
using System.Collections.Immutable;


namespace RegisterAPI.Controllers
{
    [ApiController]
    [Authorize]
    //[Authorize(Policy = "ReadOnlyAccess")]
    [Route("api/user")]
    public class UserController : Controller
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        
        [HttpGet]
        //[Authorize(Roles = "Api.ReadOnly")] for role based access
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(){

            var users = await  _userService.GetAllUsersAsync();

            return Ok(users);

        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]

        public async Task<ActionResult<UserDto>> Authorize([FromBody] LoginDto login)
        {

            if (login == null || string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.PasswordHash))
                return BadRequest("Invalid input");

            var user = await _userService.Authenticate(login.UserName, login.PasswordHash);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDto>> RegisterUsers(UserRegistrationDto userRegistrationDto ) { 

            bool result = await _userService.RegisterUserAsync(userRegistrationDto);
            if (result)
            {
                return CreatedAtAction(nameof(GetUsers), new { id = userRegistrationDto.Email, userRegistrationDto });
            }
           
            return BadRequest();        
        }       
    }

    public class LoginDto
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }


    public class SortArrayRequest
        {
            public int[] Array { get; set; }
            public bool IsAscending { get; set; }
        }    
}
