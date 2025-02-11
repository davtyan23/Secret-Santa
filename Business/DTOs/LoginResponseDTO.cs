using Business.DTOs;

namespace Business.DTOs
{
    public class LoginResponseDTO
    {
        public readonly object Email;

        public bool IsSuccess { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public UserPassDto UserPass { get; set; } 
        public int UserId {  get; set; }
        public string PassHash { get; set; }
    }
}
