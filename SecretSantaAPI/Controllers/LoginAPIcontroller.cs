using Business;
using Business.DTOs.Request;
using Microsoft.AspNetCore.Mvc;
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
            var result =  await _authService.SignInAsync(request);

            return Ok(result);
            //if (result.Succeeded)
            //{
            //    return Ok("Login successful");
            //}
            //return Unauthorized("Invalid email or password");
        }
    }
}

