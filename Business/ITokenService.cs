namespace Business.Services
{
    public interface ITokenService
    {
        string CreateToken(string userId,string role);
    }
}