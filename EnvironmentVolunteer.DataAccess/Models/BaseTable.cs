using System;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentVolunteer.DataAccess.Models
{
    public class BaseTable
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
