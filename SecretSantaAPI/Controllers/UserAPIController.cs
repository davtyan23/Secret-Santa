using Business;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaAPI.Controllers;
using Business.Services;
using Business.DTOs.Request;
using Azure.Core;

namespace SecretSantaAPI.Controllers
{
    public class UserAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly ILoggerAPI _loggerAPI; 
        private readonly IEmailSender _emailSender;
        
        public UserAPIController(IAuthService authService, IRepository repository, ITokenService tokenService, ILoggerAPI logger, IEmailSender emailSender)
        {
            _authService = authService;
            _repository = repository;
            _tokenService = tokenService;
            _loggerAPI = logger;
            _emailSender = emailSender;
            
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly ILoggerAPI _loggerAPI;
    private readonly IRepository _repository;
    private readonly IEmailSender _emailSender;
    private readonly IAuthService _authService;
    public SampleController(IRepository repository, ILoggerAPI logger, IEmailSender emailSender, IAuthService authService)
    {
        _repository = repository;
        _loggerAPI = logger;
        _emailSender = emailSender;
        _authService = authService;
    }

    [HttpPut("ConfirmationCodeSender")]
   /* public async Task<IActionResult> ResetPassword([FromBody] string email)
    {
        //  email = _repository.GetUserByEmailAsync(email);
        int confirmation = Generate6Numbers();

        var user = await _repository.GetUserByEmailAsync(email);
        if (user.Id == null || user.Id == 0)
        {
            return NotFound("user not found");
        }
        var existingConfirmation = await _repository.GetPasswordResetCodeByEmailAsync(email);
        if (existingConfirmation != null)
        {
            existingConfirmation.ConfirmationCode = confirmation.ToString();
            existingConfirmation.ExpirationTime = DateTime.Now.AddMinutes(15);
      //      await _repository.UpdatePasswordResetAsync(existingResetRequest);
        }
        else
        {
            
            var passwordReset = new PasswordReset
            {
                UserId = user.Id,
                ConfirmationCode = confirmationCode,
                ExpirationTime = DateTime.Now.AddMinutes(15), 
            };

            await _repository.CreatePasswordResetAsync(passwordReset);
        }
        string subject = "Password Reset Confirmation Code";
        string message = $"Your confirmation code for resetting your password is: {confirmation}";
        await _emailSender.SendEmailAsync(user.Email, subject, message);
        return Ok(new { Message = "Confirmation code sent successfully to your email." });

    }*/

    [HttpPost("VerifyConfirmCode")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequestDTO request)
    {
        var isValid = await _authService.VerifyConfirmationCodeAsync(request.Email, request.ConfirmationCode);
        if (!isValid)
        {
            return BadRequest(new { message = "Invalid or expired confirmation code." });
        }

        return Ok(new { message = "Confirmation code is valid." });
    }

    private int Generate6Numbers()
    {
        Random random = new Random();
        return random.Next(100000, 999999);
    }

    [HttpPost("AssignedRoles")]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignDTO request)
    {
        _loggerAPI.Info($"Assigning Role: UserId={request.UserId}, RoleId={request.RoleId}");
        try
        {
            var assignedRole = await _repository.RoleAssigning(request.UserId, request.RoleId);

            return Ok(new
            {
                message = "Role has been assigned successfully.",
                AssignedRole = assignedRole
            });
        }
        catch (Exception ex)
        {
            _loggerAPI.Error($"Error while assigning role: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while assigning the role." });
        }
    }
}




// [HttpPost("UserMethods")]
//  public async Task<IActionResult> UserEvent([FromBody] UserAPIController request)
// {
//    var userRole = await _repository.GetUserRoleById(user.Id);
//    var token = await _tokenService.CreateToken(user.UserId, userRole);
//    return Ok(new { Token = token, UserId = request.UserId, UserRole = userRole });
//   }

//  }
//}
