using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


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
        public bool UserIsAuthenticated => User.Identity?.IsAuthenticated ?? false;
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                Response.Redirect("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!UserIsAuthenticated)
            {
                return RedirectToPage("/Login"); // Redirect unauthenticated users to login
            }

            // Get user ID from claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var parsedUserId))
            {
                return RedirectToPage("/Error"); // Redirect if user ID is missing or invalid
            }

            // Validate the token and get the group ID
            var groupId = await _repository.ValidateTokenAsync(Token);
            if (groupId == null)
            {
                return RedirectToPage("/Error"); // Redirect if the token is invalid or group not found
            }

            // Safely use the groupId by casting it to int
            var success = await _repository.AddUserToGroupAsync(parsedUserId, groupId.Value);
            if (!success)
            {
                return RedirectToPage("/Error"); // Redirect if adding the user fails
            }

            return RedirectToPage("/GroupDetails", new { id = groupId }); // Redirect to the group details page
        }



    }
}
