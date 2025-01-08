using System;
//using System.Data.Entity;
using DataAccess.Models;
using DataAccess.UserViewModels;

using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class Repository : IRepository
    {
        private readonly SecretSantaContext _context;
        private readonly ILoggerAPI _loggerAPI;

        public Repository(SecretSantaContext context, ILoggerAPI loggerAPI)
        {
            _context = context;
            _loggerAPI = loggerAPI;
        }

        public Task<List<User>> GetPaginatedUsersAsync(int limit, int offset)
        {
            return _context.Users
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
        public async Task AddUserPassesAsync(UserPass user)
        {
            var a = user.PassHash.Count();
            user.PassHash = user.PassHash;
            await _context.UserPasses.AddAsync(user);
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.AddAsync(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
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
        
        public async Task<string> GetRoleById(RoleIdEnum id)
        {
            var role = await _context.Roles.FindAsync(id);
            var roleName = role == null ? null : role.RoleName;
            return roleName;
        }


        public async Task<List<User>> GetActiveUsersAsync(int limit, int offset)
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public int IsEmailRegistered(string email)
        {
            bool isEmailRegistered = true;
            var user = _context.UserPasses.Where(u => u.Email == email);
            isEmailRegistered = user.Count() > 0;
            return 0;
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            User result = _context.Users.FirstOrDefault(u => u.UserPass.Email == email);
            if (result == null)
            {
                return new User { };
            }
            else return result;
        }

        public async Task<UserPass> GetUserPassByEmailAsync(string email)
        {
            UserPass result = _context.UserPasses.FirstOrDefault(u => u.Email == email);
            if (result == null)
            {
                return new UserPass { };
            }
            else return result;
        }

        public async Task<AssignedRole> GetRoleByUserIdAsync(int userId)
        {
            return await _context.AssignedRoles
                .FirstOrDefaultAsync(ar => ar.UserId == userId) ?? new AssignedRole();
        }

        public async Task<AssignedRole> RoleAssigning(int userId, RoleIdEnum roleId)
        {
            var role = await _context.Roles.FindAsync(roleId); // maybe int convert
            if (role == null)
            {
                _loggerAPI.Error($"Role with ID {roleId} not found.");
                throw new Exception("Not existing role");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                _loggerAPI.Error($"User with ID {userId} not found.");
                throw new Exception("User does not exist");
            }

            var assignedRole = await _context.AssignedRoles
                .FirstOrDefaultAsync(ar => ar.UserId == userId && ar.RoleId == roleId);

            if (assignedRole != null)
            {
                _loggerAPI.Warn($"User ID {userId} already has role ID {roleId} assigned.");
                return assignedRole;
            }

            var newAssignedRole = new AssignedRole
            {
                UserId = userId,
                RoleId = (RoleIdEnum)roleId
            };

            await _context.AssignedRoles.AddAsync(newAssignedRole);
            await _context.SaveChangesAsync();
            return newAssignedRole;
        }

        public async Task<PassResetConfiramtionCode?> GetPasswordResetCodeByEmailAsync(string email)
        {
            return await _context.PasswordResetConfirmationCodes
                .FirstOrDefaultAsync(x => x.UserEmail == email);
        }

        public async Task AddConfirmationCodeAsync(PassResetConfiramtionCode code)
        {
            await _context.PasswordResetConfirmationCodes.AddAsync(code);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveConfirmationCodeAsync(string email)
        {
            var code = await _context.PasswordResetConfirmationCodes
                              .FirstOrDefaultAsync(c => c.UserEmail == email);

            if (code != null)
            {
                _context.PasswordResetConfirmationCodes.Remove(code);
                await _context.SaveChangesAsync();
            }
        }

        // public async Task PasswordReset
        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new DataAccess.UserViewModels.UserViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Email = u.UserPass.Email,
                    IsActive = u.IsActive,
                    RegisterTime = u.RegisterTime
                })
                .ToListAsync();

        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            var user = await _context.UserPasses.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        public async Task<List<Group>> GetGroupsAsync(int userId)
        {
            return await _context.UsersGroups
                .Where(ug => ug.UserID == userId)
                .Select(ug => ug.Groups)
                .ToListAsync();
        }

    }
}
