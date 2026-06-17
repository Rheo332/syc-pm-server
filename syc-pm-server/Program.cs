using Microsoft.EntityFrameworkCore;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Application.Security;
using syc_pm_server.Application.UseCases;
using syc_pm_server.Domain.Entities;
using syc_pm_server.Infrastructure.Persistence;
using syc_pm_server.Infrastructure.Repositories;
using syc_pm_server.Infrastructure.Security;
using syc_pm_server.Infrastructure.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//PostgreSQL EF Core
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

    // TODO: das hier bitte irgendwann rausnehmen und nicht vergessen
    // aus testzwecken: immer einen admin user mit passwort 1234 anlegen, wenn er nicht existiert. In production sollte das natürlich anders gehandhabt werden
    if (db.Users.Any(u => u.Username == "admin"))
    {
        var adminUser = db.Users.First(u => u.Username == "admin");
        db.Users.Remove(adminUser);
    }

    var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

    db.Users.Add(new User
    {
        Id = Guid.NewGuid(),
        Username = "admin",
        PasswordSalt = salt,
        PasswordHash = hasher.Hash("1234", salt)
    });

    await db.SaveChangesAsync();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
