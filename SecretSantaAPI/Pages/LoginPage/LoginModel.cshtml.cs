using Business;
using Business.DTOs.Request;
using Business.Services;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

using DataAccess.UserViewModels;

namespace SecretSantaAPI.Pages.LoginPage
{
    public class LoginModel : PageModel
    {
        private readonly SecretSantaContext _context;
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public LoginModel(IAuthService authService, IRepository repository, ITokenService tokenService, IUserService userService, SecretSantaContext context)
        {
            _authService = authService;
            _repository = repository;
            _tokenService = tokenService;
            this._userService = userService;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string EmailError { get; set; } = string.Empty;
        public string PasswordError { get; set; } = string.Empty;

        // New properties for group data
        public bool IsGroupOwner { get; set; }

        // List of groups the user participates in
        public List<Group> ParticipatingGroups { get; set; } = new();

        // List of groups the user has created (owns)
        public List<Group> CreatedGroups { get; set; } = new();

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new UserViewModel();

        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }

        public string Token { get; set; }
        public string Message { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public async Task OnGet()
        {
            Input = new InputModel();

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            // Check if the user is the owner of any group
            IsGroupOwner = await _context.Groups
                .AnyAsync(g => g.OwnerUserID == userId);

            // Fetch participating groups where the user is a member
            ParticipatingGroups = await _context.UserGroups
                .Where(ug => ug.UserID == userId)
                .Select(ug => ug.Groups)
                .ToListAsync();

            // Fetch groups created by the user (as an owner)
            CreatedGroups = await _context.Groups
                .Where(g => g.OwnerUserID == userId)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var input = await Request.ReadFromJsonAsync<InputModel>();
            if (input == null || string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
            {
                return new JsonResult(new { IsSuccess = false, ErrorMsg = "Email and password are required." });
            }

            // Authenticate the user using the input
            var user = await _userService.AuthenticateAsync(input.Email, input.Password);
            if (user == null)
            {
                return new JsonResult(new { IsSuccess = false, ErrorMsg = "Invalid login attempt." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.UserPass.Email),
            };

            foreach (var role in user.AssignedRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleId.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return new JsonResult(new { IsSuccess = true, RedirectUrl = "/User/UserPageModel?token=token&userId=" + user.Id + "&message=Login successful" });
        }


        public async Task<IActionResult> OnPostAsync2()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userPass = await _authService.SignInAsync(new LoginRequestDTO
            {
                 Email = Input.Email,
                Password = Input.Password
            });

            if (userPass == null || userPass.UserId == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var users = await _repository.GetUsersByIdAsync(new List<int> { userPass.UserId });
            var user = users.FirstOrDefault();
            var role = await _repository.GetRoleByUserIdAsync(userPass.UserId);
            var roleName = await _repository.GetRoleById(role.RoleId);

            if (user == null || role == null || string.IsNullOrEmpty(roleName))
            {
                return BadRequest("User or role data is invalid.");
            }

            var token = _tokenService.CreateToken(userPass.UserId.ToString(), roleName);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1) 
            });

            return RedirectToPage("/User/UserPageModel", new { token, userPass.UserId, message = "Login successful" });
        }
    }
}