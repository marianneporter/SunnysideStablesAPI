using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SunnysideStablesAPI.Models.Identity
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
