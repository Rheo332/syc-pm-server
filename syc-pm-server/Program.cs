using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_CHANGE_ME_NOW_WITH_ENOUGH_BITS") // TODO: muss noch in eine Konfigurationsdatei ausgelagert werden
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

    var existingUsers = db.Users.ToList();
    if (existingUsers.Count == 0)
    {
        // TODO: das hier bitte irgendwann rausnehmen und nicht vergessen
        // aus testzwecken: user anlegen, wenn er nicht existiert.
        var usersToSeed = new[] { ("admin", "1234"), ("testuser", "password") };

        foreach (var (username, password) in usersToSeed)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                db.Users.Remove(existingUser);
                await db.SaveChangesAsync();
            }

            // Key derivation die auf Client Seite passieren wird, nachdem man das password eingibt und den salt vom server bekommen hat
            var masterSalt = RandomNumberGenerator.GetBytes(16);
            var masterKey = Rfc2898DeriveBytes.Pbkdf2(password, masterSalt, 10000, HashAlgorithmName.SHA256, 32);

            var authKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, Encoding.UTF8.GetBytes("auth"));
            var dataKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, Encoding.UTF8.GetBytes("data"));

            var dbSalt = RandomNumberGenerator.GetBytes(16);

            // das ist der hash, der dann wieder an den server geschickt wird zum authentifizieren
            using var hmac = new HMACSHA256(dbSalt);
            var finalHash = hmac.ComputeHash(authKey);

            // RSA schlüssel paar, das auf dem server gespeichert wird (privater schlüssel mit dataKey verschlüsselt)
            // das passiert nur ein mal beim "registrieren", danach wird der encrypted key einfach an den user geschickt
            // und er kann ihn dann entschlüsseln
            using var rsa = RSA.Create(2048);
            var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            var publicKeyStr = Convert.ToBase64String(rsa.ExportRSAPublicKey());

            // AES zum verschlüsseln (AES-GCM Mode)
            var nonce = new byte[12]; // GCM standard nonce size ist 12 bytes
            RandomNumberGenerator.Fill(nonce);

            var tag = new byte[16]; // GCM standard tag size ist 16 bytes
            var ciphertext = new byte[privateKeyBytes.Length];

            using (var aesGcm = new AesGcm(dataKey, tag.Length))
            {
                aesGcm.Encrypt(nonce, privateKeyBytes, ciphertext, tag);
            }

            // Nonce + Tag + Ciphertext konkatenieren
            var combinedData = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, combinedData, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, combinedData, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, combinedData, nonce.Length + tag.Length, ciphertext.Length);

            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordSalt = Convert.ToBase64String(dbSalt),
                PasswordHash = Convert.ToBase64String(finalHash),
                Pbkdf2Salt = Convert.ToBase64String(masterSalt),
                PublicKey = publicKeyStr,
                EncryptedPrivateKey = Convert.ToBase64String(combinedData)
            });
        }

        await db.SaveChangesAsync();
    }

    // Create seed PwEntries
    var existingEntries = db.PwEntries.ToList();
    if (existingEntries.Count == 0)
    {
        db.PwEntries.RemoveRange(existingEntries);
        await db.SaveChangesAsync();


        var entriesToSeed = new[]
        {
            new { Title = "My Google Account", Url = "https://google.com", Username = "my_email@gmail.com", Password = "google_password123", Description = "Main google account" },
            new { Title = "GitHub", Url = "https://github.com", Username = "dev_user", Password = "github_password456", Description = "Work github account" }
        };

        var allUsers = db.Users.ToList();

        foreach (var entryInfo in entriesToSeed)
        {
            // 1. Generate Data Encryption Key (DEK) for the entry
            var entryDek = new byte[32];
            RandomNumberGenerator.Fill(entryDek);

            // 2. Encrypt the entry password with the DEK using AES-GCM
            var nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            var tag = new byte[16];
            var passwordBytes = Encoding.UTF8.GetBytes(entryInfo.Password);
            var ciphertext = new byte[passwordBytes.Length];

            using (var aesGcm = new AesGcm(entryDek, tag.Length))
            {
                aesGcm.Encrypt(nonce, passwordBytes, ciphertext, tag);
            }

            var combinedPwData = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, combinedPwData, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, combinedPwData, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, combinedPwData, nonce.Length + tag.Length, ciphertext.Length);

            var pwEntry = new PwEntry
            {
                Id = Guid.NewGuid(),
                Title = entryInfo.Title,
                Url = entryInfo.Url,
                Username = entryInfo.Username,
                Description = entryInfo.Description,
                EncryptedPassword = Convert.ToBase64String(combinedPwData)
            };
            db.PwEntries.Add(pwEntry);

            // 3. Create PwEntryAccess for each user
            foreach (var user in allUsers)
            {
                // Encrypt the DEK with the user's public key
                var publicKeyBytes = Convert.FromBase64String(user.PublicKey);
                using var userRsa = RSA.Create();
                userRsa.ImportRSAPublicKey(publicKeyBytes, out _);

                var encryptedDek = userRsa.Encrypt(entryDek, RSAEncryptionPadding.OaepSHA256);

                var access = new PwEntryAccess
                {
                    PwEntryId = pwEntry.Id,
                    UserId = user.Id,
                    EncryptedEntryKey = Convert.ToBase64String(encryptedDek)
                };
                db.PwEntryAccesses.Add(access);
            }
        }

        await db.SaveChangesAsync();
    }
    // *******************************************************************************************
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
