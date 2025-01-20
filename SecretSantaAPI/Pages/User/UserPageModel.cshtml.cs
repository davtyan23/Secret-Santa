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
        public UserPageModel(SecretSantaContext context, IRepository repository, ILoggerAPI loggerAPI, IConfiguration configuration)
        {
            _context = context;
            _repository = repository;
            _loggerAPI = loggerAPI;
            _configuration = configuration;
        }

        public bool IsGroupOwner { get; set; }
        public List<Group> ParticipatingGroups { get; set; } = new List<Group>();
        public List<Group> CreatedGroups { get; set; } = new List<Group>();

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new UserViewModel();

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        public string Token { get; set; }
        public string Message { get; set; }
        public async Task OnGet()
        {
            // Redirect if the user is not authenticated
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToPage("/Account/Login");
                Console.WriteLine("No auth?");
                return;
            }

            // Extract user ID from claims
            string idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

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

            Console.WriteLine($"User info populated: {UserViewModel.FirstName}, {UserViewModel.LastName}, {UserViewModel.PhoneNumber}");
            Console.WriteLine($"TOKENNNNNN: {UserViewModel.LastName}, {UserViewModel.PhoneNumber}");

            // Retrieve groups where the user participates
            ParticipatingGroups = await _context.UsersGroups
                .Where(ug => ug.UserID == userId)
                .Select(ug => ug.Groups)
                .ToListAsync();

            Console.WriteLine($"Participating groups count: {ParticipatingGroups.Count}");

            // Retrieve groups created by the user (if needed)
            CreatedGroups = await _context.Groups
                .Where(g => g.OwnerUserID == userId)
                .ToListAsync();

            Console.WriteLine($"Created groups count: {CreatedGroups.Count}");

            // Determine if the user is the owner of any group
            IsGroupOwner = CreatedGroups.Count > 0;  // Use Count > 0 instead of Any()
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

        public async Task<IActionResult> OnPostAddGroupAsync(string groupName, string? groupDescription, string? groupLocation, int minBudget, int maxBudget)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Pages/Login");
            }

            // Get the logged-in user's ID from the JWT token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(groupName) || minBudget < 0 || maxBudget < minBudget)
            {
                ModelState.AddModelError(string.Empty, "Invalid group details provided.");
                return Page();
            }

            // Generate a JWT token for the invitation link
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("groupId", Guid.NewGuid().ToString()), // group ID
            new Claim("ownerId", userId.ToString()), // Owner ID
        }),
                Expires = DateTime.UtcNow.AddDays(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var invitationToken = tokenHandler.WriteToken(token);

            // Create a new group entity
            var newGroup = new Group
            {
                GroupName = groupName,
                GroupDescription = groupDescription,
                GroupLocation = groupLocation,
                MinBudget = minBudget,
                MaxBudget = maxBudget,
                OwnerUserID = userId,
                InvitationToken = invitationToken
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }

}