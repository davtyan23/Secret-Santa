using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccess.UserViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.Repositories;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Business;

namespace SecretSantaAPI.Pages.User
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime RegisterTime { get; set; }

        public virtual ICollection<AssignedRole> AssignedRoles { get; set; } = new List<AssignedRole>();

        public virtual UserPass? UserPass { get; set; }
    }

    public class UserPageModel : PageModel
    {
        private readonly SecretSantaContext _context;
        private readonly IRepository _repository;
        private readonly ILoggerAPI _loggerAPI;
        private readonly IConfiguration _configuration;
        private readonly SecretSantaService _secretSantaService;
        public UserPageModel(SecretSantaContext context, IRepository repository, ILoggerAPI loggerAPI, IConfiguration configuration, SecretSantaService secretSantaService)
        {
            _context = context;
            _repository = repository;
            _loggerAPI = loggerAPI;
            _configuration = configuration;
            _secretSantaService = secretSantaService;
        }
        public List<Group> Groups { get; set; }
        public bool IsGroupOwner { get; set; }
        public List<Group> ParticipatingGroups { get; set; } = new List<Group>();
        public List<Group> CreatedGroups { get; set; } = new List<Group>();

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new UserViewModel();

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

        [BindProperty]
        public string InvitationTokens { get; set; }
        public async Task OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToPage("/Account/Login");
                Console.WriteLine("No auth?");
                return;
            }

            // Extract user ID from claims
            string idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int userId))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            // Retrieve user information
            var userInfo = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UserPass)
                .Select(user => new
                {
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.UserPass.Email,
                })
                .FirstOrDefaultAsync();

            if (userInfo != null)
            {
                UserViewModel = new UserViewModel
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    PhoneNumber = userInfo.PhoneNumber,
                    UserPass = new UserPass
                    {
                        Email = userInfo.Email
                    }
                };
            }

            CreatedGroups = await _context.Groups
                .Where(g => g.OwnerUserID == userId)
                .ToListAsync() ?? new List<Group>();

            InvitationTokens = CreatedGroups.FirstOrDefault()?.InvitationToken;

            Console.WriteLine($"Selected Invitation Token: {InvitationTokens}");
            Console.WriteLine($"Created groups count: {CreatedGroups.Count}");

            ParticipatingGroups = await _context.UserGroups
                .Where(ug => ug.UserID == userId)
                .Select(ug => ug.Groups)
                .ToListAsync();

            Console.WriteLine($"Participating groups count: {ParticipatingGroups.Count}");

            Console.WriteLine("Generated Links for Created Groups:");
            foreach (var group in CreatedGroups)
            {
                if (!string.IsNullOrEmpty(group.InvitationToken))
                {
                    var generatedLink = $"{Request.Scheme}://{Request.Host}/Groups/JoinGroup?token={group.InvitationToken}";
                    Console.WriteLine($"Group: {group.GroupName}, Link: {generatedLink}");
                }
                else
                {
                    Console.WriteLine($"Group: {group.GroupName}, No token available.");
                }
            }

            IsGroupOwner = CreatedGroups.Count > 0;

            Groups = CreatedGroups;
        }



        //public async Task<IActionResult> OnPostAddOwnerToUsersAsync(int groupId, int ownerId)
        //{
        //    try
        //    {
        //        var group = await _context.Groups.FindAsync(groupId);
        //        if (group == null)
        //        {
        //            _loggerAPI.Error($"Group with ID {groupId} not found."); // Log error
        //            ModelState.AddModelError(string.Empty, "Group not found."); // Add error for UI
        //            return Page();
        //        }

        //        // Logic to add the owner to participants...
        //        var owner = await _context.Users.FindAsync(ownerId);
        //        if (owner == null)
        //        {
        //            _loggerAPI.Error($"User with ID {ownerId} not found.");
        //            ModelState.AddModelError(string.Empty, "Owner not found.");
        //            return Page();
        //        }

        //        if (!_context.GroupParticipants.Any(gp => gp.GroupId == groupId && gp.UserId == ownerId))
        //        {
        //            _context.GroupParticipants.Add(new GroupParticipant
        //            {
        //                GroupId = groupId,
        //                UserId = ownerId
        //            });
        //            await _context.SaveChangesAsync();
        //            _loggerAPI.Info($"Owner with ID {ownerId} added to group {group.GroupName} successfully.");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Owner is already a participant.");
        //        }

        //        return RedirectToPage(); // Refresh the current page
        //    }
        //    catch (Exception ex)
        //    {
        //        _loggerAPI.Error($"An error occurred: {ex.Message}"); // Log exception
        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again."); // Inform user
        //        return Page();
        //    }
        //}

        public async Task<IActionResult> OnPostAddGroupAsync(
            string groupName,
            string? groupDescription,
            string? groupLocation,
            int minBudget,
            int maxBudget,
            int CreatedGroups,
            Group group
            )
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/LoginPage/Login");
            }

            // Get the logged-in user's ID from the JWT token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/LoginPage/Login");
            }

            if (string.IsNullOrWhiteSpace(groupName) || minBudget < 0 || maxBudget < minBudget)
            {
                ModelState.AddModelError(string.Empty, "Invalid group details provided.");
                return Page();
            }

            // Create a new group entity without an invitation token for now
            var newGroup = new Group
            {
                GroupName = groupName,
                GroupDescription = groupDescription,
                GroupLocation = groupLocation,
                MinBudget = minBudget,
                MaxBudget = maxBudget,
                OwnerUserID = userId
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync(); 

            // Generate a JWT token 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("groupId", newGroup.GroupID.ToString()),
            new Claim("ownerId", userId.ToString()) 
              }),
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            newGroup.InvitationToken = tokenHandler.WriteToken(token);

         
            _context.Groups.Update(newGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(string InvitationToken)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    _loggerAPI.Warn("Unauthorized access attempt to start draw");
                    return RedirectToPage("/Account/Login");
                }

                if (string.IsNullOrEmpty(InvitationToken))
                {
                    _loggerAPI.Warn("Token missing");
                    return RedirectToPage();
                }

                // Get user ID from claims
                
                string idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idClaim, out int userId))
                {
                    _loggerAPI.Warn("Invalid user ID in claims");
                    return RedirectToPage("/Account/Login");
                }

                Group group = await _context.Groups.FirstOrDefaultAsync(g => g.InvitationToken == InvitationToken);
                if (group == null)
                {
                    _loggerAPI.Warn($"Group not found for token: {InvitationToken}");
                    return RedirectToPage();
                }

                bool isOwner = await _context.Groups
                .AnyAsync(g => g.OwnerUserID == userId);

                if (!isOwner)
                {
                    _loggerAPI.Warn($"User {userId} is not the owner of the group {group.GroupID}");
                    return RedirectToPage();
                }

                // Perform the Secret Santa draw
                await _secretSantaService.PerformDrawAsync(InvitationToken);
                _loggerAPI.Info($"Draw was done successfully for group {group.GroupID}");

        
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _loggerAPI.Error($"Error while performing the draw: {ex.Message}");
                return Page();
            }
        }


    }
}