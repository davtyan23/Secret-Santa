using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IRepository
    {
        Task<List<User>> GetPaginatedUsersAsync(int limit, int offset);
        List<User> GetAllActiveUsersAsync();
        Task AddUserAsync(UserPass user);
        Task UpdateUserAsync(User user);
        Task DeactivateUserAsync(int id);
        Task GetActiveUsersAsync(int limit, int offset);
        Task<User> GetUsersByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
    }
}
