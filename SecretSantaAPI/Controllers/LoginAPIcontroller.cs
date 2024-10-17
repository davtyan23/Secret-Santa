using Business;
using Business.DTOs.Request;
using DataAccess.Repositories;
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
        private readonly IRepository _repository;

        public LoginController(IAuthService authService, IRepository repository)
        {
            _authService = authService;
            _repository = repository;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            int isValid = _authService.IsValidEmail(request.Email);
            switch (isValid)
            {
                case -1:

                    return BadRequest("Email field is empty");
                case -2:
                    return BadRequest("Invalid email format");
                case -3:
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
            var isValid = _authService.IsValidEmail(request.Email);
            string resp = String.Empty;
            switch (isValid) 
            {
                case -1:

                    return BadRequest("Email field is empty");
                case -2:
                    return BadRequest("Invalid email format");
                case -3:
                    return BadRequest("Invalid email format");
            }
          
            try
            {
                resp = await _authService.Register(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Registration failed: {ex.Message}" });
            }
             
            return Ok(new { UserId = resp, Message = "Register was successful" });
        }

      
    }
}
