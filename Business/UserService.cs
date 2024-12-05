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

    public Task<List<User>> GetPaginatedUsersAsync(int limit, int offset)
    {
        return _repository.GetPaginatedUsersAsync(limit, offset);
    }


    public List<User> GetAllActiveUsersAsync()
    {
        var result = _repository.GetAllActiveUsersAsync();
        return result;
    }

    public Task<User> GetUsersByIdAsync(int id)
    {
        return _repository.GetUsersByIdAsync(id);
    }

    public Task AddUsersAsync(User user)
    {
        return _repository.AddUserAsync(user);
    }

    public Task UpdateUsersAsync(User user)
    {
        return _repository.UpdateUserAsync(user);
    }

    public Task GetActiveUsersAsync(int limit, int offset)
    {
        return _repository.GetActiveUsersAsync(limit, offset);
    }

    async Task IUserService.DeactivateUsersAsync(int id)
    {
        await _repository.DeactivateUserAsync(id);
    }
}
