using EnvironmentVolunteer.Core.ApiModels;
using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.ApiModels.AuthenModels;
using EnvironmentVolunteer.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.Service.Implementation
{
    public class TokenHandlerService : ITokenHandlerService
    {
        private UserManager<User> _userManager;
        private AppSettings _appSettings;

        public TokenHandlerService(UserManager<User> userManager, AppSettings appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings;
        }

        //access token
        public async Task<string> CreateAccessToken(User user)
        {
            var tokenExpirationMinutes = _appSettings.Jwt.AccessTokenExpiresTime;

            DateTime expiresAt = DateTime.Now.AddMinutes(tokenExpirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim(JwtRegisteredClaimNames.Iss, _appSettings.Jwt.Issuer, ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(), ClaimValueTypes.DateTime, _appSettings.Jwt.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _appSettings.Jwt.Audience, ClaimValueTypes.String, _appSettings.Jwt.Audience),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToString("yyyy/MM/dd hh:mm:ss"), ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim("userId", user.Id.ToString(), ClaimValueTypes.String, _appSettings.Jwt.Issuer)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Jwt.Issuer,
                Audience = _appSettings.Jwt.Audience,
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = expiresAt,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(accessToken);
        }

        //refresh token
        public async Task<string> CreateRefreshToken(User user)
        {
            string issuer = _appSettings.Jwt.Issuer;
            string audience = _appSettings.Jwt.Audience;
            string signatureKey = _appSettings.Jwt.Key;

            var refreshTokenExpirationHours = _appSettings.Jwt.RefreshTokenExpiresTime;

            DateTime expiresAt = DateTime.Now.AddHours(refreshTokenExpirationHours);

            string refreshTokenCode = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim(JwtRegisteredClaimNames.Iss, issuer, ClaimValueTypes.String,  _appSettings.Jwt.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(), ClaimValueTypes.DateTime, _appSettings.Jwt.Issuer),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToString("yyyy/MM/dd hh:mm:ss"), ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim(ClaimTypes.SerialNumber, refreshTokenCode, ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, _appSettings.Jwt.Issuer),
                new Claim("userId", user.Id.ToString(), ClaimValueTypes.String, _appSettings.Jwt.Issuer)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signatureKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Jwt.Issuer,
                Audience = _appSettings.Jwt.Audience,
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = expiresAt,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var refreshToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(refreshToken);
        }

        //Get principal from expired token
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key)),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

