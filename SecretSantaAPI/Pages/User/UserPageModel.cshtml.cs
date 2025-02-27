using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.Repositories;
using DataAccess;
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
        public int? UserId { get; set; }
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

            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            _loggerAPI.Info($"User claims: {string.Join(", ", claims)}");

            // Extract user ID from claims
            string idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int userId))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            groupInfos = await (from gi in _context.GroupsInfo
                                join u in _context.Users on gi.ReceiverID equals u.Id
                                select new GroupInfoViewModel
                                {
                                    GroupInfoID = gi.GroupInfoID,
                                    UserGroupID = gi.UserGroupID,
                                    ReceiverID = gi.ReceiverID,
                                    ReceiverFirstName = u.FirstName,
                                    ReceiverLastName = u.LastName
                                }).ToListAsync();

            Console.WriteLine($"Populated groupInfos with {groupInfos.Count} records.");

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

        public class GroupInfoViewModel
        {
            public int GroupInfoID { get; set; }
            public int UserGroupID { get; set; }
            public int ReceiverID { get; set; }
            public string ReceiverFirstName { get; set; }
            public string ReceiverLastName { get; set; }
        }

        public List<GroupInfoViewModel> groupInfos { get; set; }

        [IgnoreAntiforgeryToken]
        public JsonResult OnPostStartDraw(string invitationToken)
        {
            var group = _context.Groups.FirstOrDefault(g => g.InvitationToken == invitationToken);

            if (group == null)
            {
                return new JsonResult(new { success = false, message = "Group not found." });
            }

            var participants = _context.UserGroups
                .Where(ug => ug.GroupID == group.GroupID)
                .Select(ug => ug.UserID)
                .ToList();

            if (participants.Count < 3)
            {
                return new JsonResult(new { success = false, message = "Not enough participants to start the draw." });
            }

            return new JsonResult(new { success = true, message = "Draw successfully started!" });
        }



        public async Task<IActionResult> OnPostAsync(string InvitationToken, int? ReceiverId)
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

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState)
                    {
                        foreach (var err in error.Value.Errors)
                        {
                            _loggerAPI.Warn($"ModelState error - Key: {error.Key}, Message: {err.ErrorMessage}");
                        }
                    }
                    return Page();
                }

                bool isOwner = await _context.Groups
                .AnyAsync(g => g.OwnerUserID == userId);

                if (!isOwner)
                {
                    _loggerAPI.Warn($"User {userId} is not the owner of the group {group.GroupID}");
                    return RedirectToPage();
                }

                int participantsCount = await _context.UserGroups.CountAsync(ug => ug.GroupID == group.GroupID);

                if (participantsCount < 3)
                {
                    _loggerAPI.Warn($"Not enough participants in this group {group.GroupID} to perform the draw");

                    // Store the warning message
                    TempData["DrawWarning"] = "There must be at least 3 participants to perform the draw.";

                    return Page(); // Stay on the page and show the warning
                }

                // Perform the Secret Santa draw
                await _secretSantaService.PerformDrawAsync(InvitationToken);
                _loggerAPI.Info($"Draw was done successfully for group {group.GroupID}");

                // Populate groupInfos
                groupInfos = await (from gi in _context.GroupsInfo
                                    join u in _context.Users on gi.ReceiverID equals u.Id
                                    select new GroupInfoViewModel
                                    {
                                        GroupInfoID = gi.GroupInfoID,
                                        UserGroupID = gi.UserGroupID,
                                        ReceiverID = gi.ReceiverID,
                                        ReceiverFirstName = u.FirstName,
                                        ReceiverLastName = u.LastName
                                    }).ToListAsync();

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