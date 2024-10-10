using System;
using System.Data.Entity;
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
        public List<User> GetAllActiveUsersAsync()
        {
            var a = _context.Users.Where(u => u.IsActive == true);

            return a.ToList();
        }
        //public  Task<List<User>> GetAllActiveUsersAsync()
        //{
        //    Task<List<User>> resp = null;
        //    try
        //    {
        //         resp = _context.Users
        //            .Where(u => u.IsActive == true) // Use Where to filter active users
        //            .ToListAsync(); // Use ToListAsync for asynchronous execution
        //    }
        //    catch (Exception ex) 
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return resp;
        //}
        public async Task AddUserAsync(UserPass user)
        {
            await _context.UserPasses.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task AddUserAsync(UserPass user)
        {
            await _context.UserPasses.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

        }

        public async Task DeactivateUserAsync(int id)
        {
           var user = _context.Users.FindAsync(id);
           
           await _context.SaveChangesAsync();
        }


        public async Task<User> GetUsersByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }


        public Task GetActiveUsersAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

         public bool IsEmailRegistered(string email)
        {
            bool isEmailRegistered = true;
            var user = _context.UserPasses.Where(u => u.Email == email);
            isEmailRegistered = user.Count() > 0;
            return isEmailRegistered;
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
