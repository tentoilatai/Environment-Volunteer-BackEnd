using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.Service.ApiModels.AuthenModels
{
    public class JwtModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FullName { get; set; }
        public string Role {  get; set; }
    }
}
