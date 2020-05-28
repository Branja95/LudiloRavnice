using Microsoft.AspNetCore.Identity;

namespace RentVehicle.Models.IdentityUsers
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName)
        {

        }
    }
}
