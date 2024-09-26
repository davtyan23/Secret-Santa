using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models; 
using Microsoft.EntityFrameworkCore; 

namespace DataAccess.Repositories
{
    public class Repository : IRepository
    {
        private readonly SecretSantaContext _context;

        public Repository(SecretSantaContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsers(int limit, int offset)
        {
            return await _context.Users
                .Skip(offset)       
                .Take(limit)        
                .ToListAsync();     
        }

        public Task<List<User>> GetAllActiveUsersAsync() =>
        _context.Users
        .Where(u => u.IsActive)
        .OrderBy(u => u.Lastname)
        .ThenBy(u => u.Firstname)
        .ToListAsync();
    }
}
