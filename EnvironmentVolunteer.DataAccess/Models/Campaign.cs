using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Campaign : BaseTable<Guid>
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Project { get; set; } = string.Empty;

        [Required]
        public string Avatar { get; set; } = string.Empty;

        [Required]
        public string MainOwner { get; set; } = string.Empty;

        [Required]
        public string OwnerInfo { get; set; } = string.Empty;

        [Required]
        public int ProjectAmount { get; set; }
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool Status { get; set; }
    }
}
