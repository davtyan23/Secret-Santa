using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccess.UserViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc;

namespace SecretSantaAPI.Pages
{
    public class UserView : PageModel
    {
        private readonly SecretSantaContext _context;
        public UserView(SecretSantaContext context)
        {
            _context = context;
        }

        // Property to hold the list of UserViewModel objects
        public List<UserViewModel> Users { get; set; }

        // OnGet method to fetch users from the database
        public async Task OnGet(string searchQuery)
         {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(u => u.FirstName.Contains(searchQuery) ||
                                          u.LastName.Contains(searchQuery) ||
                                          u.PhoneNumber.Contains(searchQuery) ||
                                          u.UserPass.Email.Contains(searchQuery));
            }

            var usersWithEmails = await query
                .Include(u => u.UserPass)
                .Select(user => new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.UserPass.Email,
                    user.IsActive,
                    user.RegisterTime
                })
                .ToListAsync();

            Users = usersWithEmails.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                IsActive = u.IsActive,
                RegisterTime = u.RegisterTime
            }).ToList();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user != null)
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync();

                return RedirectToPage("./UserView");
            }
            catch (Exception ex)
            {
                {
                    // Log the error to the console or to a logging service
                    Console.WriteLine($"Error occurred while deleting user: {ex.Message}");
                    return StatusCode(500, "Internal Server Error: Unable to delete user.");
                }
            }
        }


    }
}
