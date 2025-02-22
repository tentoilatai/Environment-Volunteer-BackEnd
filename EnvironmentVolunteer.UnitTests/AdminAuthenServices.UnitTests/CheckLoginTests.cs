using AutoMapper;
using EnvironmentVolunteer.Core.Enums;
using EnvironmentVolunteer.Core.Exceptions;
using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.Implementation;
using EnvironmentVolunteer.Service.Interfaces;
using EnvironmentVolunteer.UnitTests;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanBank.UnitTests.AdminAuthenServices.UnitTests
{
    public class CheckLoginTests : BaseUnitTest
    {
        private IAdminAuthenService _adminAuthenService;

        private Mock<ITokenHandlerService> _tokenHandlerServiceMock;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            var user = new User
            {
                UserName = "admin"
            };
            var password = "Envi2025@2025@2025";

            _userManagerMock.Setup(x => x.FindByNameAsync(user.UserName)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);
            _tokenHandlerServiceMock = new Mock<ITokenHandlerService>();
            _adminAuthenService = new AdminAuthenService(_appSettingsMock.Object, _unitOfWorkMock.Object, _userContextMock.Object, _userManagerMock.Object,
                                                        _signInManagerMock.Object, _roleManagerMock.Object, _tokenHandlerServiceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task CheckLogin_UserNotFound_ReturnIncorrectUserNamePassword()
        {
            //Arrange
            var username = "user1";
            var password = "123456";

            //Assert
            var exception = Assert.ThrowsAsync<ErrorException>(async () => await _adminAuthenService.CheckLogin(username, password));
            Assert.AreEqual(exception.StatusCode, StatusCodeEnum.A02);
        }

        [Test]
        public async Task CheckLogin_IncorrectPassword_ReturnIncorrectUserNamePassword()
        {
            //Arrange
            var username = "admin";
            var password = "123456";

            //Assert
            var exception = Assert.ThrowsAsync<ErrorException>(async () => await _adminAuthenService.CheckLogin(username, password));
            Assert.AreEqual(exception.StatusCode, StatusCodeEnum.A02);

        }

        [Test]
        public async Task CheckLogin_CorrectUserNamePassword_ReturnToken()
        {
            //Arrange
            var username = "admin";
            var password = "Envi2025@2025@2025";

            //Act
            var user = new User
            {
                UserName = username
            };

            var accessToken = "access token";
            var refreshToken = "refresh token";

            _tokenHandlerServiceMock.Setup(x => x.CreateAccessToken(It.IsAny<User>())).ReturnsAsync(accessToken);
            _tokenHandlerServiceMock.Setup(x => x.CreateRefreshToken(It.IsAny<User>())).ReturnsAsync(refreshToken);

            //Assert 
            var result = await _adminAuthenService.CheckLogin(username, password);
            Assert.AreEqual(result.AccessToken, accessToken);
            Assert.AreEqual(result.RefreshToken, refreshToken);

        }
    }
}
