using Business.DTOs.Request;

namespace Business
{
    public interface IAuthService
    {
        Task<int> SignInAsync(LoginRequestDTO login);
        Task<int> RegisterUserAsync(string email, string password);
        //Task SignOutAsync();
    }

   

}
