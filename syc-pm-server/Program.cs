using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Application.Security;
using syc_pm_server.Application.UseCases;
using syc_pm_server.Infrastructure.Persistence;
using syc_pm_server.Infrastructure.Repositories;
using syc_pm_server.Infrastructure.Security;
using syc_pm_server.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Services
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

// UseCases
builder.Services.AddScoped<CreatePwEntryUseCase>();
builder.Services.AddScoped<EditPwEntryUseCase>();
builder.Services.AddScoped<DeletePwEntryUseCase>();
builder.Services.AddScoped<GetPwEntryUseCase>();
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<PreloginUseCase>();
builder.Services.AddScoped<CreateRequestUseCase>();
builder.Services.AddScoped<GetRequestsUseCase>();
builder.Services.AddScoped<DeleteRequestUseCase>();
builder.Services.AddScoped<GrantAccessUseCase>();
builder.Services.AddScoped<GetAllUsersUseCase>();
builder.Services.AddScoped<GetUserAccessUseCase>();
builder.Services.AddScoped<RevokeAccessUseCase>();

// Repositories
builder.Services.AddScoped<IPwEntryRepository, PwEntryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();

// JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "syc-pm",
            ValidAudience = "syc-pm",

            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            )
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
