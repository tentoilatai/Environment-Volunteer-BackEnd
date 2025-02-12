using EnvironmentVolunteer.Api.AutoMapperProfiles;
using EnvironmentVolunteer.Api.Middlewares;
using EnvironmentVolunteer.Api.Utils;
using EnvironmentVolunteer.Core.ApiModels;
using EnvironmentVolunteer.DataAccess.DbContexts;
using EnvironmentVolunteer.DataAccess.Implementation;
using EnvironmentVolunteer.DataAccess.Interfaces;
using EnvironmentVolunteer.DataAccess.Models;
using EnvironmentVolunteer.Service.Implementation;
using EnvironmentVolunteer.Service.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection?.Get<AppSettings>();
builder.Services.AddSingleton(appSettings ?? new AppSettings());

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<EnvironmentVolunteerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdminAuthenService, AdminAuthenService>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddScoped<ITokenHandlerService, TokenHandlerService>();
builder.Services.AddAutoMapper(typeof(ProjectProfile));

builder.Services.AddIdentity<User, Role>(opt =>
{
    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(appSettings.Admin.FailAccountLock);
    opt.Lockout.MaxFailedAccessAttempts = appSettings != null ? appSettings.Admin.MaxAccessFailedAttempts + 1 : new AppSettings().Admin.MaxAccessFailedAttempts + 1;
    opt.User.AllowedUserNameCharacters = appSettings.AllCharacters;
    opt.User.RequireUniqueEmail = false;
}).AddRoles<Role>()
.AddEntityFrameworkStores<EnvironmentVolunteerDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appSettings.Jwt.Issuer,
        ValidAudience = appSettings.Jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnvironmentVolunteer API", Version = "v1", Description = $"Last updated at {DateTimeOffset.UtcNow}" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
             {
                  new OpenApiSecurityScheme
                  {
                      Reference = new OpenApiReference
                      {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                      },
                         Scheme = "oauth2",
                         Name = "Bearer",
                         In = ParameterLocation.Header,
                  },
                      new List<string>()
             }
        });
});

builder.Services.AddAutoMapper(typeof(ProjectProfile));

var app = builder.Build();

app.MigrateDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthenMiddleware>();
app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

app.Run();
