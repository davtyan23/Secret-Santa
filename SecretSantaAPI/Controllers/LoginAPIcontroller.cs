using Business;
using Business.DTOs.Request;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace SecretSantaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            bool isValid = IsValidEmail(request.Email);
            if (!isValid)
            {
                return BadRequest("Invalid email format");
            }
                
            var userId = await _authService.SignInAsync(request);
            if (userId == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new { UserId = userId, Message = "Login successful" });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(Business.RegisterRequest request)
        {
            bool isValid = IsValidEmail(request.Email);
            if (!isValid)
            {
                return BadRequest("Invalid email format");
            }
                
            var userId = await _authService.RegisterUserAsync( request);
            if (userId == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new { UserId = userId, Message = "Login successful" });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
