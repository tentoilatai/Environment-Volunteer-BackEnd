using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class User : IdentityUser<Guid>
    {
        public string NameProfile { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLogin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
