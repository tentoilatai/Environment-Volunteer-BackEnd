using EnvironmentVolunteer.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Role : IdentityRole<Guid>
    {
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
