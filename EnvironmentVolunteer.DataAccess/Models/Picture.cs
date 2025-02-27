using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Picture : BaseTable<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Image { get; set; } = string.Empty ;

    }
}
