using Business;
using DataAccess;
using DataAccess.Models;
using DataAccess.DTOs;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaAPI.Controllers;
using Business.Services;
using Business.DTOs.Request;

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
    public SampleController(IRepository repository, ILoggerAPI logger, IEmailSender emailSender)
    {
        _repository = repository;
        _loggerAPI = logger;
        _emailSender = emailSender;
    }

    [HttpPut("ResetPassword")]
    public async Task<IActionResult> ResetPassw([FromBody]string Email)
    {
   
        int confirmation = Generate6Numbers();
        var user = await _repository.GetUserByEmailAsync(Email);
        if (user == null)
        {
            return NotFound("user not found");
        }
        string subject = "Password Reset Confirmation Code";
        string message = $"Your confirmation code for resetting your password is: {confirmation}";

        return Ok(new { Message = "Confirmation code sent successfully to your email." });
    }

    private int Generate6Numbers()
    {
        Random random = new Random();

        return random.Next(100000, 999999);
    }
    [HttpPost("AssignedRoles")]
    [Authorize(Policy = "ParticipantPolicy")]

    // Protect this controller or action
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
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
