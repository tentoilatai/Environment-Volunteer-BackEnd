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
using System.Reflection.Metadata.Ecma335;

namespace EnvironmentVolunteer.Service.Implementation
{
    public class AdminAuthenService : BaseService, IAdminAuthenService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ITokenHandlerService _tokenService;
        public AdminAuthenService(
            AppSettings appSettings,
            IUnitOfWork unitOfWork,
            UserContext userContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            ITokenHandlerService tokenService,
            IMapper mapper
        ) : base(appSettings, unitOfWork, userContext, mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        //sign up
        public async Task<string> RegisterAccount(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                NameProfile = model.FullName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.B01);
            }

            return "Sign up successfully";
        }

        //log in
        public async Task<JwtModel> CheckLogin(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || username.Any(char.IsUpper) || !await _userManager.CheckPasswordAsync(user, password))
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.A02);
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
                throw new ErrorException(Core.Enums.StatusCodeEnum.A02);
            }
        }

        public async Task<JwtModel> RefreshTokenAsync(string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);
            var username = principal.Identity.Name; // this is mapped to the Name claim by default
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.A02);
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
