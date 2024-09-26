using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IRepository
    {
        Task<List<User>> GetUsers(int limit, int offset);
    }
}
