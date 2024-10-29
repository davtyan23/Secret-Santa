using Business.DTOs.Request;
using DataAccess.Models;
using DataAccess.Repositories;

namespace Business
{
    public interface IAuthService
    {
        Task<UserPass> SignInAsync(LoginRequestDTO login);
        Task<string> Register(RegisterRequest request);
        int IsValidEmail(string email);
        bool VerifyPass(string enteredPassword, string storedHash);
    }

   

}
