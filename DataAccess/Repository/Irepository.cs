using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IRepository
    {
        Task AddUserAsync(User user);
        Task DeactivateUserAsync(int id);
        Task DeleteUserAsync(int id);
        Task<List<User>> GetAllActiveUsersAsync();
        Task<List<User>> GetPaginatedUsersAsync(int limit, int offset);
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
    }
}
