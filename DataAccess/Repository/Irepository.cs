using DataAccess.Models;
using DataAccess.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DataAccess.Repositories
{
    public interface IRepository
    {
        Task<List<User>> GetPaginatedUsersAsync(int limit, int offset);
        List<User> GetAllActiveUsersAsync();
        Task<User> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeactivateUserAsync(int id);
        Task<List<User>> GetActiveUsersAsync(int limit, int offset);
        Task<User> GetUsersByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<UserPass> GetUserPassByEmailAsync(string email);
        Task<AssignedRole> GetRoleByUserIdAsync(int userId);
        Task<AssignedRole> RoleAssigning(int userId, RoleIdEnum roleId);
        Task<string> GetRoleById(RoleIdEnum id);
        Task<PassResetConfiramtionCode?> GetPasswordResetCodeByEmailAsync(string email);
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task AddUserPassesAsync(UserPass newUserPass);
        // Task UpdatePasswordResetAsync(PasswordReset passwordReset);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> AddUserToGroupAsync(int userId, int groupId);
        Task<int?> ValidateTokenAsync(string token);
        Task<Group?> GetGroupByTokenAsync(string invitationToken);
        string GenerateInvitationToken(int groupId);
        Task<Group> CreateGroupAsync(Group group);

    }
}
