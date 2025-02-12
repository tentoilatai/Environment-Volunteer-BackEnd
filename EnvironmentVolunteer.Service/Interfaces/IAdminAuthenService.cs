using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.ApiModels.AuthenModels;
using System.Threading.Tasks;
using EnvironmentVolunteer.Service.ApiModels;
using System;
using Microsoft.AspNetCore.Mvc;

namespace EnvironmentVolunteer.Service.Interfaces
{
    public interface IAdminAuthenService
    {
        Task<JwtModel> CheckLogin(string username, string password);
        Task<bool> LogoutAsync();
        Task<JwtModel> RefreshTokenAsync(string refreshToken);
    }
}
