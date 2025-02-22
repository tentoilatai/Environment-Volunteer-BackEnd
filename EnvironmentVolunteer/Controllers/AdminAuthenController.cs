using EnvironmentVolunteer.Core.ApiModels;
using EnvironmentVolunteer.Core.Exceptions;
using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.ApiModels.AuthenModels;
using EnvironmentVolunteer.Service.Interfaces;
using Azure.Core;
using Azure.Identity;
using EnvironmentVolunteer.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace EnvironmentVolunteer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthenController : BaseApiController
    {
        public IAdminAuthenService _adminAuthenService { get; set; }
        private readonly IConfiguration _configuration;
        public AdminAuthenController(IServiceProvider serviceProvider, IAdminAuthenService adminAuthenService, IConfiguration configuration) : base(serviceProvider)
        {
            _adminAuthenService = adminAuthenService;
            _configuration = configuration;
        }

        [HttpPost("signUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] RegisterModel registerModel)
        {
            var registerUser = await _adminAuthenService.RegisterAccount(registerModel);
            if (registerUser == null)
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.BadRequest);
            }
            return Success(registerUser);

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var loginUser = await _adminAuthenService.CheckLogin(loginModel.Username, loginModel.Password);

            if (loginUser == null)
            {
                return Unauthorized();
            }

            return Success(loginUser);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _adminAuthenService.LogoutAsync();
            return Success("Logout successful");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenApiModel refreshTokenApiModel)
        {
            try
            {
                var jwtModel = await _adminAuthenService.RefreshTokenAsync(refreshTokenApiModel.RefreshToken);
                return Success(jwtModel);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
