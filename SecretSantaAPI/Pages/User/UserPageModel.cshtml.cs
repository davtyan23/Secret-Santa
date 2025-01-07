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

        public UserPageModel(SecretSantaContext context) => _context = context;

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new UserViewModel();

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        public string Token { get; set; }
        public string Message { get; set; }

        public async Task OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                 RedirectToPage("/Account/Login");  // Redirect if not authenticated
                Console.WriteLine("No auth?");
                return;
            }

            string idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            if (!int.TryParse(idClaim, out int identity))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            int id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");

            var userInfo = await _context.Users.
                   Where(u => u.Id == id).
                   Include(u => u.UserPass).
                   Select(user => new
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

            //public async Task<IActionResult> OnPostAsync(int id)
            //{
            //   return 
            //}
        }
    }
}