using EnvironmentVolunteer.Core.ApiModels;
using EnvironmentVolunteer.DataAccess.Interfaces;
using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.Interfaces;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace EnvironmentVolunteer.UnitTests
{
    public class BaseUnitTest
    {
        public Mock<AppSettings> _appSettingsMock;
        public Mock<IUnitOfWork> _unitOfWorkMock;
        public Mock<IMapper> _mapperMock;
        public Mock<UserContext> _userContextMock;
        public Mock<UserManager<User>> _userManagerMock;
        public Mock<SignInManager<User>> _signInManagerMock;
        public Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        public Mock<RoleManager<Role>> _roleManagerMock;
        public Mock<IBackgroundJobClient> _backgroundJobClientMock;
        public Mock<IAdminAuthenService> _adminAuthenServiceMock;

        public static Guid guid0 = Guid.Empty;
        public static Guid guid1 = Guid.NewGuid();
        public static Guid guid2 = Guid.NewGuid();
        public static Guid guid3 = Guid.NewGuid();
        public static Guid guid4 = Guid.NewGuid();
        public static Guid guid5 = Guid.NewGuid();
        public static Guid guid6 = Guid.NewGuid();
        public static Guid guid7 = Guid.NewGuid();
        public static Guid guid8 = Guid.NewGuid();
        public static Guid guid9 = Guid.NewGuid();
        public static Guid guid10 = Guid.NewGuid();
        public static Guid guid11 = Guid.NewGuid();
        public static Guid guid12 = Guid.NewGuid();
        public static Guid guid13 = Guid.NewGuid();
        public static Guid guid14 = Guid.NewGuid();
        public static Guid guid15 = Guid.NewGuid();
        public static Guid guid16 = Guid.NewGuid();
        public static Guid guid17 = Guid.NewGuid();
        public static Guid guid18 = Guid.NewGuid();
        public static Guid guid19 = Guid.NewGuid();
        public static Guid guid20 = Guid.NewGuid();
        public static Guid guid21 = Guid.NewGuid();
        public static Guid guid22 = Guid.NewGuid();
        public static Guid guid23 = Guid.NewGuid();
        public static Guid guid24 = Guid.NewGuid();
        public static Guid guid25 = Guid.NewGuid();
        public static Guid guid26 = Guid.NewGuid();
        public static Guid guid27 = Guid.NewGuid();
        public static Guid guid28 = Guid.NewGuid();
        public static Guid guid29 = Guid.NewGuid();
        public static Guid guid30 = Guid.NewGuid();
        public static Guid guid31 = Guid.NewGuid();
        public static Guid guid32 = Guid.NewGuid();
        public static Guid guid33 = Guid.NewGuid();
        public static Guid guid34 = Guid.NewGuid();
        public static Guid guid99 = Guid.NewGuid();

        public BaseUnitTest()
        {
            //user manager
            var userStoreMock = new Mock<IUserStore<User>>();
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<User>>();
            var userValidators = new List<IUserValidator<User>>(); // If you have any validators, you can create and add them here
            var passwordValidators = new List<IPasswordValidator<User>>(); // Same as above for validators
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<User>>>();
            var webHostEnvironmentMock = InitMockService<IWebHostEnvironment>();
            var backgroundJobClientMock = InitMockService<IBackgroundJobClient>();
            var adminAuthenService = InitMockService<IAdminAuthenService>();


            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                optionsAccessorMock.Object,
                passwordHasherMock.Object,
                userValidators,
                passwordValidators,
                keyNormalizerMock.Object,
                errorsMock.Object,
                servicesMock.Object,
                loggerMock.Object
            );



            //var signInManagerMock = new FakeSignInManager();
            var signInManagerMock = new Mock<SignInManager<User>>(
                       new FakeUserManager(),
                       new Mock<IHttpContextAccessor>().Object,
                       new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                       new Mock<IOptions<IdentityOptions>>().Object,
                       new Mock<ILogger<SignInManager<User>>>().Object,
                       new Mock<IAuthenticationSchemeProvider>().Object,
                       new Mock<IUserConfirmation<User>>().Object
                       );


            // role manager
            var roleManagerMock = new Mock<RoleManager<Role>>(
                    new Mock<IRoleStore<Role>>().Object,
                    new IRoleValidator<Role>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<ILogger<RoleManager<Role>>>().Object
                    );

            var appSettingsMock = new Mock<AppSettings>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            var userContextMock = new Mock<UserContext>();

            _appSettingsMock = appSettingsMock;
            _unitOfWorkMock = unitOfWorkMock;
            _mapperMock = mapperMock;
            _userContextMock = userContextMock;
            _userManagerMock = userManagerMock;
            _roleManagerMock = roleManagerMock;
            _signInManagerMock = signInManagerMock;
            _webHostEnvironmentMock = webHostEnvironmentMock;
            _backgroundJobClientMock = backgroundJobClientMock;
            _adminAuthenServiceMock = adminAuthenService;
        }

        public Mock<T> InitMockService<T>() where T : class
        {
            return new Mock<T>();
        }
    }


    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager()
                : base(new FakeUserManager(),
                       new Mock<IHttpContextAccessor>().Object,
                       new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                       new Mock<IOptions<IdentityOptions>>().Object,
                       new Mock<ILogger<SignInManager<User>>>().Object,
                       new Mock<IAuthenticationSchemeProvider>().Object,
                       new Mock<IUserConfirmation<User>>().Object)
        { }
    }

    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<User>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<User>>().Object,
        new IUserValidator<User>[0],
              new IPasswordValidator<User>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<User>>>().Object)
        { }

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}
