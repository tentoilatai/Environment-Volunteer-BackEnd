using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Project : BaseTable<Guid>
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Logo { get; set; } = string.Empty ;


        [Required]
        public string Description { get; set; } = string.Empty ;

        [Required]
        public int Duration { get; set; } 

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int LimitedMember { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
