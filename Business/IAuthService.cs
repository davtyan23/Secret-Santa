using Business.DTOs;
using Business.DTOs.Request;
using DataAccess.Models;

namespace Business
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> SignInAsync(LoginRequestDTO login);
        Task<string> Register(RegisterRequestDTO request);
        int IsValidEmail(string email);
        bool VerifyPass(string enteredPassword, string storedHash);
        Task<bool> VerifyConfirmationCodeAsync(string email,string confirmationCode);
    }
}
