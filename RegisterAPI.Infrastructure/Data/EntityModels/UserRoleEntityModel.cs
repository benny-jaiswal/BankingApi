using RegisterAPI.Core.Model;

namespace RegisterAPI.Infrastructure.Data.EntityModels
{
    public class UserRoleEntityModel
    {
        public int UserRoleId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation Properties
        public UserEntityModel User { get; set; }
        public RoleEnityModel Role { get; set; }
    }
}
