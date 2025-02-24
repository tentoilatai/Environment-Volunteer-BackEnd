using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Participant : BaseTable<Guid>
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
