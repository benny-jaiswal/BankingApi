using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RegisterAPI.Core.Model;
using RegisterAPI.Infrastructure.Data;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Application.Services;
using RegisterAPI.Model.Core.Model.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegisterAPI.Application.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IConfiguration config, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _context = context;
            _config = config;
            _contextAccessor = httpContext;
            _mapper = mapper;
        }

        public async Task<UserDto> Authenticate(string username, string password)
        {
            UserEntityModel user;
            // Use case-insensitive comparison if using InMemory (for testing)
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => EF.Functions.Collate(u.UserName, "SQL_Latin1_General_CP1_CI_AS") == username);
            }

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null; // Invalid credentials

            var userDto = _mapper.Map<UserDto>(user);
            // Generate JWT token
            var token = GenerateJwtToken(userDto);
            userDto.Token = token;

            return userDto;
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

        public List<string> GetUserRoles(int userId)
        {
            return _context.UserRoles.Where(u => u.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.RoleName)
                .ToList();
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
                UserName = userRegistrationDto?.FirstName + userRegistrationDto?.LastName,
                FirstName = userRegistrationDto?.FirstName,
                LastName = userRegistrationDto?.LastName,
                Email = userRegistrationDto.Email,
                PasswordHash = userRegistrationDto.Password
            };

            _context.Users.Add(user);
            var success = await _context.SaveChangesAsync();
            return success > 0;       
                
        }

        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int accountId, int count)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    AccountId = t.AccountId,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();
        }

        // ✅ Generate Financial Summary
        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(int accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync();

            return new FinancialSummaryDto
            {
                TotalDeposits = transactions.Where(t => t.TransactionType == "Deposit").Sum(t => t.Amount),
                TotalWithdrawals = transactions.Where(t => t.TransactionType == "Withdraw").Sum(t => t.Amount),
                NetBalance = transactions.Sum(t => t.Amount)
            };
        }

        private string GenerateJwtToken(UserDto user)
        {
            var claims = new List<Claim>
            { 
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = GetUserRoles(user.UserId);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return password == storedHash;

            //return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
