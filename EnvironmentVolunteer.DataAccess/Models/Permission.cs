using EnvironmentVolunteer.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Permission : BaseTable<Guid>
    {
        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, StringLength(255)]
        public string Feature { get; set; }

        [Required]
        public string NormalizedName { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
