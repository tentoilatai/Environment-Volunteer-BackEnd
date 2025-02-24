using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Sponsor : BaseTable<Guid>
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ContactInfo { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string ContributionType { get; set; }   // Loại đóng góp (Tiền mặt, vật phẩm, dịch vụ,...)

        public string Notes { get; set; } = string.Empty;

    }
}
