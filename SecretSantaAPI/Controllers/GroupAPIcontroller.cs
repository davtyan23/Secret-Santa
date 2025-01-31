using Business.Services;
using Microsoft.EntityFrameworkCore;
using Business;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Models;
using System.Text.RegularExpressions;


namespace SecretSantaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupAPIcontroller : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly SecretSantaContext _context;
        public GroupAPIcontroller(IAuthService authService, IRepository repository, ITokenService tokenService, SecretSantaContext context)
        {
            _authService = authService;
            _repository = repository;
            _tokenService = tokenService;
            _context = context;
        }

       
        [HttpGet("get-group-by-token")]
      //  [Authorize(Policy = "ParticipantPolicy")]
        // Ensure only authenticated users can access this endpoint
        public async Task<IActionResult> GetGroupByTokenAsync([FromQuery] string invitationToken)
        {
            if (string.IsNullOrWhiteSpace(invitationToken))
            {
                return BadRequest("Invitation token is required.");
            }

            var userIdClaim = User.FindFirst("id")?.Value; // Get the user's ID from claims
            var userRole = User.FindFirst("role")?.Value; // Get the user's role from claims

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            var group = await _context.Groups
               .FirstOrDefaultAsync(g => g.InvitationToken == invitationToken); 

            if (group == null)
            {
                return NotFound("Invalid or expired invitation token.");
            }

            // Perform ownership or role check
            if (group.OwnerUserID != userId && userRole != "Participant" && userRole != "Admin")
            {
                return Forbid("User is not authorized to view this group.");
            }


            return Ok(new
            {
                group.GroupID,
                group.GroupName,
                group.GroupDescription,
                group.GroupLocation,
                group.MinBudget,
                group.MaxBudget
            });
        }

        [HttpGet("Get-invitationLink")]
      //  [Authorize("Owner")]
        public async Task<IActionResult> GetInvitationLinkAsync([FromQuery] int groupID) {
            var group = await _context.Groups.FindAsync(groupID);
            if (group == null)
            {
                return NotFound("Group not found");
            }
                var invitationLink = GetInvitationLinkAsync(groupID);
            
            return Ok(new { InvitationLink = invitationLink });
        }

        [HttpPost("join-group")]
        public async Task<IActionResult> JoinGroup([FromQuery] string token)
        {
            // Validate the invitation token and fetch the group
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.InvitationToken == token);
            if (group == null)
            {
                return NotFound("Invalid or expired invitation token.");
            }

            // Get the logged-in user's ID from the JWT claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Check if the user is already in the group
            var existingMembership = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.UserID == userId && ug.GroupID == group.GroupID);

            if (existingMembership != null)
            {
                return BadRequest("You are already a member of this group.");
            }

            // Add the user to the group
            var userGroup = new UserGroup
            {
                UserID = userId,
                GroupID = group.GroupID
            };

            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            return Ok("You have successfully joined the group.");
        }


    }

}

