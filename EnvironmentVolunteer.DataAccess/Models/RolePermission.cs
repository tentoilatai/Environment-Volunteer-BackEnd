using EnvironmentVolunteer.DataAccess.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class RolePermission : BaseTable<Guid>
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid PermissionId { get; set; }

        public virtual Role Role { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
