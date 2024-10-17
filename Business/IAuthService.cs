using Business.DTOs.Request;
using DataAccess.Models;
using DataAccess.Repositories;

namespace Business
{
    public interface IAuthService
    {
        Task<int> SignInAsync(LoginRequestDTO login);
        Task<string> Register(RegisterRequest request);
        int IsValidEmail(string email);

    }

   

}
