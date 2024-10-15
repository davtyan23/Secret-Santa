using Business.DTOs.Request;
using DataAccess.Models;

namespace Business
{
    public interface IAuthService
    {
        Task<int> SignInAsync(LoginRequestDTO login);
        //Task<string> RegisterUserAsync(string email, string password);
        //Task<User> RegisterUserAsync(RegisterRequest request);
        Task<User> RegisterUserAsync(RegisterRequest request);
        int IsValidEmail(string email);
        //Task SignOutAsync();
    }

   

}
