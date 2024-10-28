using Business;
using DataAccess.DTOs;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaAPI.Controllers;

namespace SecretSantaAPI.Controllers
{
    public class UserAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly ILoggerAPI _loggerAPI; 

        public UserAPIController(IAuthService authService, IRepository repository, ITokenService tokenService, ILoggerAPI logger)
        {
            _authService = authService;
            _repository = repository;
            _tokenService = tokenService;
            _loggerAPI = logger; 
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Participant")]
public class SampleController : ControllerBase
{
    private readonly ILoggerAPI _loggerAPI;
    private readonly IRepository _repository;
    public SampleController(IRepository repository, ILoggerAPI logger)
    {
         _repository = repository;
        _loggerAPI = logger;
    }

    [HttpGet("Data")]
    public IActionResult GetData()
    {
        var name = User.Identity.Name;
        var customClaim = User.FindFirst("CustomClaim")?.Value;
        var idClaim = User.FindFirst("id")?.Value;

        _loggerAPI.Info($"User Name: {name}, Custom Claim: {customClaim}, ID Claim: {idClaim}");

        return Ok(new
        {
            Message = "Here is your data",
            Id = idClaim,
            Name = name,
            CustomClaim = customClaim
        });
    }

    [HttpPost("AssignedRoles")]
    [Authorize(Roles = "Participant")] // Protect this controller or action
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignDTO request)
    {
        try
        {
            var assignedRole = await _repository.RoleAssigning(request.UserId, request.RoleId); // call the method from your repository
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
