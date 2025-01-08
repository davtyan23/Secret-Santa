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

        public UserPageModel(SecretSantaContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public List<Group> ParticipatingGroups { get; set; } = new();
        public List<Group> CreatedGroups { get; set; } = new();

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

            // Retrieve groups where the user participates
            ParticipatingGroups = await _context.UsersGroups
                .Where(ug => ug.UserID == userId)
                .Select(ug => ug.Groups)
                .ToListAsync();

            Console.WriteLine($"Participating groups count: {ParticipatingGroups.Count}");

            // Optionally, retrieve groups created by the user (if needed)
            CreatedGroups = await _context.Groups
                .Where(g => g.OwnerUserID == userId)
                .ToListAsync();

            Console.WriteLine($"Created groups count: {CreatedGroups.Count}");
        }

        public async Task<IActionResult> OnPostAddGroupAsync(string groupName, string? groupDescription, string? groupLocation,int minBudget,int maxBudget)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Pages/Login");
            }
            // Get the logged-in user's ID
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
            // Create a new group entity
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

            return RedirectToPage();
        }
    }
    
}