using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class GroupInfo : BaseTable<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime StartDay { get; set; }
        [Required]
        public DateTime EndDay { get; set; }
        public string Schedule { get; set; } = string.Empty;
    }
}
