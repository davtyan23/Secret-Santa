using DataAccess.Models;
using DataAccess.UserViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace SecretSantaAPI.Pages.Admin
{
    public class EditUser : PageModel
    {
        private readonly SecretSantaContext _context;

        public EditUser(SecretSantaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserViewModel User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserPass)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            User = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.UserPass?.Email,
                IsActive = user.IsActive,
                RegisterTime = user.RegisterTime
            };

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.FindAsync(User.Id);

            if (user == null)
            {
                return NotFound();
            }

            var userPass = await _context.UserPasses
                             .Where(u => u.UserId == User.Id)
                             .FirstOrDefaultAsync();

            //user.UserPass = new UserPass()
            //{
            //    Email = "azuri@gmail.com"
            //};

            //if (user.UserPass == null)
            //{
            //    ModelState.AddModelError(string.Empty, "UserPass details are missing.");
            //    return Page();
            //}
            user.FirstName = User.FirstName;
            user.LastName = User.LastName;
            user.PhoneNumber = User.PhoneNumber;
            if (userPass != null)
            {
                userPass.Email = User.Email;
            }
            //user.IsActive = User.IsActive;
            _context.Users.Update(user);
            _context.UserPasses.Update(userPass);
            await _context.SaveChangesAsync();

            return RedirectToPage("/UserView");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.FindAsync(User.Id);
            if (user == null)
            {
                return NotFound();
            }
            var userPass = await _context.UserPasses
                            .Where(u => u.UserId == User.Id)
                            .FirstOrDefaultAsync();
            if (userPass != null)
            {
                _context.UserPasses.Remove(userPass);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            Console.WriteLine("DELETE");

            return RedirectToPage("/UserView");
        }

        public async Task<IActionResult> OnPostIsActiveAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToPage("/UserView");
        }
    }
}

