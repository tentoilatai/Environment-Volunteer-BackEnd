using System;

namespace EnvironmentVolunteer.Core.ApiModels
{
    public class UserContext
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public Guid? ActionId { get; set; }
    }
}
