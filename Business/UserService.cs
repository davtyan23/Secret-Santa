using Business;
using DataAccess.Models;
using DataAccess.Repositories;

public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<User>> GetPaginatedUsersAsync(int limit, int offset)
    {
        return await _repository.GetPaginatedUsersAsync(limit, offset);
    }

    public async Task<List<User>> GetAllActiveUsersAsync()
    {
        return await _repository.GetAllActiveUsersAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _repository.GetUserByIdAsync(id);
    }

    public async Task AddUserAsync(User user)
    {
        await _repository.AddUserAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _repository.UpdateUserAsync(user);
    }

    public async Task DeactivateUserAsync(int id)
    {
        await _repository.DeactivateUserAsync(id);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _repository.DeleteUserAsync(id);
    }
}
