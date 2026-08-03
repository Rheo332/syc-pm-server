using Microsoft.EntityFrameworkCore;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;
using syc_pm_server.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace syc_pm_server.Infrastructure.Repositories;

public class PwEntryRepository : IPwEntryRepository
{
    private readonly AppDbContext _db;

    public PwEntryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PwEntryAccess>> GetUserEntriesAsync(Guid userId)
    {
        return await _db.PwEntryAccesses
            .Include(ea => ea.PwEntry)
            .Where(ea => ea.User.Id == userId)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(PwEntry pwEntry, Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null || user.Username != "admin") // muss noch angepasst werden, um die Admin-Rolle zu überprüfen
        {
            return false;
        }

        // 1. Generate Data Encryption Key (DEK) for the entry
        var entryDek = new byte[32];
        RandomNumberGenerator.Fill(entryDek);

        // 2. Encrypt the entry password with the DEK using AES-GCM
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        var tag = new byte[16];
        var passwordBytes = Encoding.UTF8.GetBytes(pwEntry.EncryptedPassword);
        var ciphertext = new byte[passwordBytes.Length];

        using (var aesGcm = new AesGcm(entryDek, tag.Length))
        {
            aesGcm.Encrypt(nonce, passwordBytes, ciphertext, tag);
        }

        var combinedPwData = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, combinedPwData, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combinedPwData, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, combinedPwData, nonce.Length + tag.Length, ciphertext.Length);

        var newPwEntry = pwEntry;
        newPwEntry.EncryptedPassword = Convert.ToBase64String(combinedPwData);
        _db.PwEntries.Add(newPwEntry);
        await _db.SaveChangesAsync();

        var publicKeyBytes = Convert.FromBase64String(user!.PublicKey);
        using var userRsa = RSA.Create();
        userRsa.ImportRSAPublicKey(publicKeyBytes, out _);

        var encryptedDek = userRsa.Encrypt(entryDek, RSAEncryptionPadding.OaepSHA256);

        var access = new PwEntryAccess
        {
            PwEntryId = pwEntry.Id,
            UserId = user.Id,
            EncryptedEntryKey = Convert.ToBase64String(encryptedDek)
        };
        _db.PwEntryAccesses.Add(access);
        await _db.SaveChangesAsync();
        return true;
    }
}
