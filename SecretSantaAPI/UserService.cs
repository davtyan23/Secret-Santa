using Business.Services;
using Business;
using DataAccess.Models;
using DataAccess.Repositories;
using Business.DTOs.Request;

public interface IUserService
{
    Task<User> AuthenticateAsync(string email, string password);
}
public class UserService : IUserService
{
    private readonly IAuthService _authService;
    private readonly IRepository _repository;
    private readonly ITokenService _tokenService;

    public UserService(IAuthService authService, IRepository repository, ITokenService tokenService)
    {
        _authService = authService;
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        // Authenticate user using _authService
        var userPass = await _authService.SignInAsync(new LoginRequestDTO
        {
            Email = email,
            Password = password
        });

        if (userPass == null || userPass.UserId == 0)
        {
            return null; // Invalid credentials
        }

        // Fetch user details and role
        var user = await _repository.GetUsersByIdAsync(userPass.UserId);
        var role = await _repository.GetRoleByUserIdAsync(userPass.UserId);
        var roleName = await _repository.GetRoleById(role.RoleId);

        if (user == null || role == null || string.IsNullOrEmpty(roleName))
        {
            throw new InvalidOperationException("User or role data is invalid.");
        }

        // Generate a token for the user
        var token = _tokenService.CreateToken(userPass.UserId.ToString(), roleName);

        // Set token in the user object for additional use
        //user.Token = token;

        return user;
    }
}
