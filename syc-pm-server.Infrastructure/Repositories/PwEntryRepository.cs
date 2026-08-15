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
        if (user == null || user.Username != "admin")
        {
            return false;
        }

        var entryDek = new byte[32];
        RandomNumberGenerator.Fill(entryDek);

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

    public async Task<bool> DeleteAsync(Guid entryId, Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null || user.Username != "admin")
        {
            return false;
        }

        var entry = await _db.PwEntries.FindAsync(entryId);
        if (entry == null) return false;

        var accesses = await _db.PwEntryAccesses.Where(a => a.PwEntryId == entryId).ToListAsync();
        _db.PwEntryAccesses.RemoveRange(accesses);

        _db.PwEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(PwEntry pwEntry, Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null || user.Username != "admin")
        {
            return false;
        }

        var existing = await _db.PwEntries.FindAsync(pwEntry.Id);
        if (existing == null) return false;

        existing.Title = string.IsNullOrEmpty(pwEntry.Title) ? existing.Title : pwEntry.Title;
        existing.Url = string.IsNullOrEmpty(pwEntry.Url) ? existing.Url : pwEntry.Url;
        existing.Username = string.IsNullOrEmpty(pwEntry.Username) ? existing.Username : pwEntry.Username;
        existing.Description = string.IsNullOrEmpty(pwEntry.Description) ? existing.Description : pwEntry.Description;

        // new DEK if password changed
        if (!string.IsNullOrEmpty(pwEntry.EncryptedPassword) && existing.EncryptedPassword != pwEntry.EncryptedPassword)
        {
            var entryDek = new byte[32];
            RandomNumberGenerator.Fill(entryDek);

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

            existing.EncryptedPassword = Convert.ToBase64String(combinedPwData);

            var accesses = await _db.PwEntryAccesses
                .Include(a => a.User)
                .Where(a => a.PwEntryId == pwEntry.Id)
                .ToListAsync();

            foreach (var access in accesses)
            {
                var pubKeyBytes = Convert.FromBase64String(access.User.PublicKey);
                using var rsa = RSA.Create();
                rsa.ImportRSAPublicKey(pubKeyBytes, out _);

                var encDek = rsa.Encrypt(entryDek, RSAEncryptionPadding.OaepSHA256);
                access.EncryptedEntryKey = Convert.ToBase64String(encDek);
            }
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> GrantAccessAsync(Guid entryId, Guid adminUserId, Guid targetUserId, string encryptedEntryKey)
    {
        var adminUser = await _db.Users.FindAsync(adminUserId);
        if (adminUser == null || adminUser.Username != "admin")
        {
            return false;
        }

        var entry = await _db.PwEntries.FindAsync(entryId);
        if (entry == null) return false;

        var targetUser = await _db.Users.FindAsync(targetUserId);
        if (targetUser == null) return false;

        var existingAccess = await _db.PwEntryAccesses.FirstOrDefaultAsync(a => a.PwEntryId == entryId && a.UserId == targetUserId);
        if (existingAccess != null) return true;

        var access = new PwEntryAccess
        {
            PwEntryId = entryId,
            UserId = targetUserId,
            EncryptedEntryKey = encryptedEntryKey
        };

        _db.PwEntryAccesses.Add(access);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Guid>> GetUserAccessAsync(Guid userId)
    {
        return await _db.PwEntryAccesses
            .Where(ea => ea.UserId == userId)
            .Select(ea => ea.PwEntryId)
            .ToListAsync();
    }

    public async Task<bool> RevokeAccessAsync(Guid entryId, Guid adminUserId, Guid targetUserId)
    {
        var adminUser = await _db.Users.FindAsync(adminUserId);
        if (adminUser == null || adminUser.Username != "admin")
        {
            return false;
        }

        var access = await _db.PwEntryAccesses
            .FirstOrDefaultAsync(a => a.PwEntryId == entryId && a.UserId == targetUserId);

        if (access == null) return false;

        _db.PwEntryAccesses.Remove(access);
        await _db.SaveChangesAsync();
        return true;
    }
}
