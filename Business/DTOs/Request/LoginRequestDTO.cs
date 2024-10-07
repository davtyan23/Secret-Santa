using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Request
{
    public class LoginRequestDTO
    {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
