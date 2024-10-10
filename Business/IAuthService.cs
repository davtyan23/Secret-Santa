using Business.DTOs.Request;

namespace Business
{
    public interface IAuthService
    {
        Task<int> SignInAsync(LoginRequestDTO login);
        //Task<string> RegisterUserAsync(string email, string password);
        Task<bool> RegisterUserAsync(RegisterRequest request);

        //Task SignOutAsync();
    }

   

}
