using DataAccess.Models;
using DataAccess.UserViewModels;

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
        Task<UserPass> GetUserByEmailAsync(string email);
        Task<AssignedRole> GetRoleByUserIdAsync(int userId);
        Task<AssignedRole> RoleAssigning(int userId, int roleId);
        Task<string> GetRoleById(int id);
        Task<PassResetConfiramtionCode?> GetPasswordResetCodeByEmailAsync(string email);
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task AddUserPassesAsync(UserPass newUserPass);
        // Task UpdatePasswordResetAsync(PasswordReset passwordReset);
    }
}
