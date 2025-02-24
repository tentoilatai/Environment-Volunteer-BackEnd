using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class ProjectResult : BaseTable<Guid>
    {
        [Required]
        public string Report { get; set; } = string.Empty;

        public string ApprovedBy { get; set; } = string.Empty ;
    }
}
