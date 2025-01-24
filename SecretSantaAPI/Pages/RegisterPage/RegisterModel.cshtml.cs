using Business.DTOs.Request;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SecretSantaAPI.Pages.RegisterPage
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;
        public RegisterModel(IAuthService authService,IRepository repository)
        {
            _authService = authService;
            _repository = repository;
            Input = new InputModel();  
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string PhoneNumber { get; set; }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Hors arev");
                Console.WriteLine($"FirstName: {Input.FirstName}, LastName: {Input.LastName}, Email: {Input.Email}, Password: {Input.Password}, PhoneNumber: {Input.PhoneNumber}");
                return Page();
            }

            try
            {
                // Check if the email already exists in the database
                bool isEmailTaken = await _repository.IsEmailTakenAsync(Input.Email);
                if (isEmailTaken)
                {
                    ErrorMessage = "The email address is already registered.";
                    return Page();
                }

                var registerDto = new RegisterRequestDTO
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Email = Input.Email,
                    Password = Input.Password,
                    PhoneNumber = Input.PhoneNumber
                };

                string response = await _authService.Register(registerDto);

                if (string.IsNullOrEmpty(response))
                {
                    ErrorMessage = "Registration failed. Please try again.";
                    return Page();
                }

                Message = "Registration successful!";
                return RedirectToPage("/User/UserPageModel");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
}
