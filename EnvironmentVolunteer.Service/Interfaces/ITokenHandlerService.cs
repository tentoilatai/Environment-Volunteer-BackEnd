using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.ApiModels.AuthenModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.Service.Interfaces
{
    public interface ITokenHandlerService
    {
        Task<string> CreateAccessToken(User user);
        Task<string> CreateRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
