using Microsoft.EntityFrameworkCore;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;
using syc_pm_server.Infrastructure.Persistence;

namespace syc_pm_server.Infrastructure.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _db;

        public RequestRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Request> CreateAsync(Request request)
        {
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.Username != "admin")
            {
                return false;
            }

            var req = await _db.Requests.FindAsync(id);
            if (req == null) return false;

            _db.Requests.Remove(req);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Request>> GetAllAsync(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.Username != "admin")
            {
                return new List<Request>();
            }

            return await _db.Requests.ToListAsync();
        }

        public async Task<Request?> GetByIdAsync(Guid id)
        {
            return await _db.Requests.FindAsync(id);
        }
    }
}