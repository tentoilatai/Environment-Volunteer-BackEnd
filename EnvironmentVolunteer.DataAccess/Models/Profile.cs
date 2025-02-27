using EnvironmentVolunteer.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class Profile : BaseTable<Guid>
    {
        [Required]
        public string Avatar { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [EmailAddress]
        public string PersonalEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        [RegularExpression("^[0-9]*$")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Bio { get; set; }

        [Required]
        public string Language {  get; set; } = string.Empty;

        [Required]
        public string Education { get; set; } = string.Empty;
        public string? Skill { get; set; }
        public string? Experience { get; set; }
        public string? Certification { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }


    }
}
