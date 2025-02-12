using EnvironmentVolunteer.Core.ApiModels;
using EnvironmentVolunteer.Core.Exceptions;
using EnvironmentVolunteer.DataAccess.Interfaces;
using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.ApiModels.AuthenModels;
using EnvironmentVolunteer.Service.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using EnvironmentVolunteer.Service.Implementation;
using Microsoft.AspNet.Identity;

namespace EnvironmentVolunteer.Service.Implementation
{
    public class AdminAuthenService : BaseService, IAdminAuthenService
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<Role> _roleManager;
        private readonly ITokenHandlerService _tokenService;
        public AdminAuthenService(
            AppSettings appSettings,
            IUnitOfWork unitOfWork,
            UserContext userContext,
            Microsoft.AspNetCore.Identity.UserManager<User> userManager,
            SignInManager<User> signInManager,
            Microsoft.AspNetCore.Identity.RoleManager<Role> roleManager,
            ITokenHandlerService tokenService,
            IMapper mapper
        ) : base(appSettings, unitOfWork, userContext, mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        //log in
        public async Task<JwtModel> CheckLogin(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || username.Any(char.IsUpper) || !await _userManager.CheckPasswordAsync(user, password))
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.BadRequest);
            }

            user.IsLogin = true;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            string accessToken = await _tokenService.CreateAccessToken(user);
            string refreshToken = await _tokenService.CreateRefreshToken(user);

            return new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.NameProfile
            };
        }

        //logout
        public async Task<bool> LogoutAsync()
        {
            var userId = _userContext.UserId.ToString();

            var result = await _userManager.FindByIdAsync(userId);
            if (result != null)
            {
                result.IsLogin = false;
                await _userManager.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            else
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.BadRequest);
            }
        }

        public async Task<JwtModel> RefreshTokenAsync(string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);
            var username = principal.Identity.Name; // this is mapped to the Name claim by default
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.BadRequest);
            }

            var newAccessToken = await _tokenService.CreateAccessToken(user);
            var newRefreshToken = await _tokenService.CreateRefreshToken(user);

            return new JwtModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                FullName = user.UserName
            };
        }
    }
}
