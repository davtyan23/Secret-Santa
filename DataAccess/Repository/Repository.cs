using System;
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

            public async Task<List<User>> GetPaginatedUsersAsync(int limit, int offset)
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

            public async Task addUserAsync(User user)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateUserAsync(User user)
            {
                _context.Users.Update(user);
                 await _context.SaveChangesAsync();
                
            }

            public async Task DeactivateUserAsync(int id)
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.IsActive = false;  
                    await _context.SaveChangesAsync();
                }
            }

        public Task AddUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }



    }
}
