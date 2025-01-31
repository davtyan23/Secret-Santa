using DataAccess.Models;

namespace Business
{
    public interface IUserService
    {
        Task<List<User>> GetPaginatedUsersAsync(int limit, int offset);
        List<User> GetAllActiveUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetUsersByIdAsync(List<int> userIds);
        Task<User> AddUsersAsync(User user);
        Task UpdateUsersAsync(User user);
        Task DeactivateUsersAsync(int isActive);
        Task<User> AuthenticateAsync(string email, string password);
    }
}
