using DataAccess.Models;

namespace Business
{
    public interface IUserService
    {
        Task<List<User>> GetPaginatedUsersAsync(int limit, int offset);
        Task<List<User>> GetAllActiveUsersAsync();
        Task<User> GetUsersByIdAsync(int id);
        Task AddUsersAsync(User user);
        Task UpdateUsersAsync(User user);
        Task DeactivateUsersAsync(int id);
        Task DeleteUsersAsync(int id);
     
        
    }
}
