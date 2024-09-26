using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IUserService
    {
        Task<List<User>> GetPaginatedUsersAsync(int limit, int offset);
        Task<List<User>> GetAllActiveUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeactivateUserAsync(int id);
        Task DeleteUserAsync(int id);
    }
}
