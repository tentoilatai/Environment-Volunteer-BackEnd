using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class ProjectFinance : BaseTable<Guid>
    {
        [Required]
        public string Item { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal UnitPrice { get; set; } 

        [Required]
        public int Quantity { get; set; }
    }
}
