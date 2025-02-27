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
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            _userContext = userContext;
        }

        //sign up
        public async Task<string> RegisterAccount(RegisterModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return "Email is used. Please create new email!";
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingUserByUsername != null)
            {
                return "Username is alrady taken. Please choose another one";
            }

            var user = new User
            {
                UserName = model.Username,
                NameProfile = model.FullName,
                Email = model.Email,
                EmailConfirmed = true
            };

            if (user?.Email == null || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.A01);
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(user.Email, emailPattern))
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.B04);
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                throw new ErrorException(Core.Enums.StatusCodeEnum.B02);
            }

            //check role existed before assign
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new Role { Name = "User" });
            }


            await _userManager.AddToRoleAsync(user, "User");

            return "Signed Up Successfully";
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

            var roles = await _userManager.GetRolesAsync(user);

            return new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.NameProfile,
                Role = roles.FirstOrDefault() ?? "User",
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

            var roles = await _userManager.GetRolesAsync(user);

            return new JwtModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                FullName = user.NameProfile,
                Role = roles.FirstOrDefault() ?? "User",
            };
        }
    }
}
