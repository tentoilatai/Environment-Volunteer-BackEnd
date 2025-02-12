using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace EnvironmentVolunteer.Api.Middlewares
{
    public class EnvironmentVolunteerAuthorizeAttribute : AuthorizeAttribute
    {
        public EnvironmentVolunteerAuthorizeAttribute() { }
        public EnvironmentVolunteerAuthorizeAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
