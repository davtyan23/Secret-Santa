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

    public List<User> GetAllActiveUsersAsync()
    {
        var result = _repository.GetAllActiveUsersAsync();  
        return result;
    }

    public async Task<User> GetUsersByIdAsync(int id)
    {
        return await _repository.GetUsersByIdAsync(id);
    }

    public async Task UpdateUsersAsync(User user)
    {
        await _repository.UpdateUserAsync(user);
    }
    public async Task AddUserPassesAsync(UserPass user)
    {
        await _repository.AddUserPassesAsync(user);
    }

    public async Task GetActiveUsersAsync(int limit, int offset)
    {
        await _repository.GetActiveUsersAsync(limit, offset);
    }

     async Task IUserService.DeactivateUsersAsync(int id)
    {
            await _repository.DeactivateUserAsync(id);
    }

    public Task<User> FindAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddUsersAsync(User user)
    {
        throw new NotImplementedException();
    }
}     
