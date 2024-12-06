using Business;
using Business.DTOs.Request;
using Business.Services;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace SecretSantaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        public LoginController(IAuthService authService, IRepository repository, ITokenService tokenService)
        {
            _authService = authService;
            _repository = repository;
            _tokenService = tokenService;
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
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

            var userPass = await _authService.SignInAsync(request);
            if (userPass == null || userPass.UserId == 0)
            {
                return Unauthorized("Invalid email or password");
            }

            bool isPassValid = _authService.VerifyPass(request.Password, userPass.PassHash);

            if (!isPassValid)
            {
                return Unauthorized("Invalid email or password");
            }
            var user = await _repository.GetUsersByIdAsync(userPass.UserId);
            var role = await _repository.GetRoleByUserIdAsync(userPass.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (role == null)
            {
                return BadRequest("No role assigned to this user.");
            }
            if (string.IsNullOrEmpty(role.RoleId.ToString()))
            {
                return BadRequest("Role has no name assigned.");
            }
            var email = userPass.Email;
            var userId = userPass.UserId;
            var roleName = await _repository.GetRoleById(role.RoleId);
            var token = _tokenService.CreateToken(userPass.UserId.ToString(), roleName);
            Console.WriteLine($"Role Id: {role.RoleId}");

            return Ok(new
            {
                User = new
                {
                    UserId = userId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = email

                },
                Token = token,
                Message = "Login successful"
            });
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

            var registerDto = new RegisterRequestDTO
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password
            };

            try
            {
                resp = await _authService.Register(registerDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Registration failed: {ex.Message}" });
            }

            return Ok(new { UserId = resp, Message = "Register was successful" });
        }

    }
}
public enum EmailErrorCode
    {
        EmailEmpty = -1,
        EmailInvalidFormat = -2,
    }