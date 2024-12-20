using Business;
using Business.DTOs.Request;
using Business.Services;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DataAccess.Models;
using NuGet.Common;

namespace SecretSantaAPI.Pages.LoginPage
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public LoginModel(IAuthService authService, IRepository repository, ITokenService tokenService, IUserService userService)
        {
            _authService = authService;
            _repository = repository;
            _tokenService = tokenService;
            this._userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string EmailError { get; set; } = string.Empty;
        public string PasswordError { get; set; } = string.Empty;
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

        public void OnGet()
        {
            Input = new InputModel();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Input.Email) || string.IsNullOrEmpty(Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Email and Input.Password are required.");
                return Page();
            }

            // Authenticate the user
            var user = await _userService.AuthenticateAsync(Input.Email, Input.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.UserPass.Email),
               // new Claim(ClaimTypes.Role, user.AssignedRoles.First().RoleId.ToString()) // Add role claim for authorization
            };

            foreach (var role in user.AssignedRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleId.ToString()));
            }

            // Create claims identity and principal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Redirect to the home page or specified return URL
            return RedirectToPage("/User/UserPageModel", new { token = "token", userId = user.Id, message = "Login successful" });
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

            var user = await _repository.GetUsersByIdAsync(userPass.UserId);
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
                Expires = DateTime.UtcNow.AddHours(1) // Expiry time for the token
            });

            return RedirectToPage("/User/UserPageModel", new { token, userPass.UserId, message = "Login successful" });
        }
    }
}