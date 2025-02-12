using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.Service.ApiModels.AuthenModels
{
    public class RefreshTokenApiModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
