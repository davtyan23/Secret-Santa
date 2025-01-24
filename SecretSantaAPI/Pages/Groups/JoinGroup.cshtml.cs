using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace SecretSantaAPI.Pages.Groups
{
    public class JoinGroupModel : PageModel
    {
        private readonly SecretSantaContext _context;
        private readonly IRepository _repository;
        public JoinGroupModel(SecretSantaContext context,IRepository repository)
        {
           _context = context;
           _repository = repository;
        }
        public bool UserIsAuthenticated => User.Identity.IsAuthenticated;

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        public void OnGet(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Response.Redirect("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!UserIsAuthenticated)
            {
                return RedirectToPage("Pages/LoginPage/Login");
            }

            // Get user ID from claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var parsedUserId))
            {
                return RedirectToPage("/Error");
            }

            // Validate the token and get the group ID
            var group = await _repository.GetGroupByTokenAsync(Token);
            if (group == null)
            {
                return RedirectToPage("/Error");
            }

            var success = await _repository.AddUserToGroupAsync(parsedUserId, group.GroupID);
            if (!success)
            {
                return RedirectToPage("/Error");
            }

            return RedirectToPage("/LoginPage/Login", new { id = group }); 
        }

        //public async Task<IActionResult> OnPostJoinGroupAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
