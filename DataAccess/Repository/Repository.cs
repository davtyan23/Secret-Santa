using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccess.Models;
using DataAccess.UserViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DataAccess.Repositories
{
    public class Repository : IRepository
    {
        private readonly SecretSantaContext _context;
        private readonly ILoggerAPI _loggerAPI;
        private readonly IConfiguration _configuration;

        public Repository(SecretSantaContext context, ILoggerAPI loggerAPI, IConfiguration configuration)
        {
            _context = context;
            _loggerAPI = loggerAPI;
            _configuration = configuration;
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
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FindAsync(userId);
        }
        public Task<List<User>> GetUsersByIdAsync(List<int> userIds)
        {
            return _context.Users
                                 .Where(u => userIds.Contains(u.Id)).ToListAsync();
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
            return await _context.UserGroups
                .Where(ug => ug.UserID == userId)
                .Select(ug => ug.Groups)
                .ToListAsync();
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await _context.Groups
                .FirstOrDefaultAsync(g => g.GroupID == groupId);
        }

        public async Task<List<UserGroup>> GetUserGroupAsync(int groupId)
        {
            return await _context.UserGroups.
                Where(ug => ug.GroupID == groupId).
                ToListAsync();
        }

        public async Task<bool> AddUserToGroupAsync(int userId, int groupId)
        {
            // Check if the user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return false; // Group not found
            }

            // Check if the user is already in the group
            var existingUserGroup = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.UserID == userId && ug.GroupID == groupId);
            if (existingUserGroup != null)
            {
                return false; // User is already in the group
            }

            // Add the user to the group
            var userGroup = new UserGroup
            {
                UserID = userId,
                GroupID = groupId
            };

            await _context.UserGroups.AddAsync(userGroup);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Group?> GetGroupByTokenAsync(string invitationToken)
        {
            if (string.IsNullOrWhiteSpace(invitationToken))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                // Validate token
                tokenHandler.ValidateToken(invitationToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract claims from JWT
                var jwtToken = (JwtSecurityToken)validatedToken;
                var groupId = jwtToken.Claims.FirstOrDefault(c => c.Type == "groupId")?.Value;

                if (int.TryParse(groupId, out int parsedGroupId))
                {
                    return await _context.Groups.FirstOrDefaultAsync(g => g.GroupID == parsedGroupId);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }


        public string GenerateInvitationToken(int groupId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new[]
            {
            new Claim("groupId", groupId.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(4), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            group.InvitationToken = GenerateInvitationToken(group.GroupID);

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return group;
        }


        public async Task<int?> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                // Validate the token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract groupId from claims
                var jwtToken = (JwtSecurityToken)validatedToken;
                var groupId = jwtToken.Claims.FirstOrDefault(c => c.Type == "groupId")?.Value;

                if (int.TryParse(groupId, out int parsedGroupId))
                {
                    var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupID == parsedGroupId);
                    return group?.OwnerUserID;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public async Task SaveGroupInfoAsync(List<GroupInfo> groupInfos)
        {
            await _context.GroupsInfo.AddRangeAsync(groupInfos);
            await _context.SaveChangesAsync();
        }


        /* public async Task<string> GetGroupLinkAsync(Group group)
         {
             if (string.IsNullOrEmpty(group.InvitationToken))
             {
                 throw new InvalidOperationException("Group does not have a valid invitation token.");
             }



             // Replace with your actual API domain
             return $"https://localhost:7195/join-group?token={group.InvitationToken}";
         }*/
    }
}
