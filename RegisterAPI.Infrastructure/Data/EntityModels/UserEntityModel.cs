using RegisterAPI.Infrastructure.Data.EntityModels;

namespace RegisterAPI.Core.Model
{
    public class UserEntityModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public DateTime DateOfBirth { get; set; }
        public ICollection<UserRoleEntityModel> UserRoles { get; set; } = new List<UserRoleEntityModel>();
        public ICollection<BankAccountEntityModel> BankAccounts { get; set; } = new List<BankAccountEntityModel>();
    }

}
