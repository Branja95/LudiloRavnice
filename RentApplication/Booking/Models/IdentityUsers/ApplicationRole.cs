using Microsoft.AspNetCore.Identity;

namespace Booking.Models.IdentityUsers
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName)
        {

        }
    }
}
