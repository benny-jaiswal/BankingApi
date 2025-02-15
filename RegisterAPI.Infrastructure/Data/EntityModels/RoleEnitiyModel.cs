namespace RegisterAPI.Infrastructure.Data.EntityModels
{
    public class RoleEnityModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRoleEntityModel> UserRoles { get; set; } = new List<UserRoleEntityModel>();
    }
}
