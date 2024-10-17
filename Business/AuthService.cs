using Business.DTOs.Request;
using DataAccess.Models;
using DataAccess.Repositories;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Business
{
    public class AuthService : IAuthService
    {

        //private readonly UserManager<LoginRequest> _userManager; 
        //private readonly SignInManager<LoginRequest> _signInManager; // For handling sign in

        //public AuthService(UserManager<LoginRequest> userManager, SignInManager<LoginRequest> signInManager)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //}

        //public async Task<SignInResult> SignInAsync(string email, string password)
        //{
        //    // Find user by email
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return SignInResult.Failed; // User not found
        //    }

        //    // Trying to sign in
        //    return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        //}

        //public async Task<IdentityResult> RegisterAsync(string email, string password)
        //{
        //    // Validate email and password hash
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        //    }

        //    if (string.IsNullOrWhiteSpace(password))
        //    {
        //        throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        //    }
        //    var user = new LoginRequest { Password = password, Email = email }; 
        //    return await _userManager.CreateAsync(user, password);
        //}

        //private readonly UserService _userService;
       
        
        private readonly IRepository _repository;
        public AuthService (IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));//checks its not null
        }

        public string HashPass(string password) 
        {
            using (var sha512 = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha512.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        // change input values to model 
        
        public async Task<string> Register(RegisterRequest request)
        {
            RegistrationPassCheck(request);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                RegisterTime = DateTime.Now,
                IsActive = true
            };
           
            var newUser = new UserPass
            {
                Email = request.Email,
                PassHash = HashPass(request.Password),
                //CreatedAt = DateTime.Now
            };
            var resp = await _repository.AddUserAsync(user);
            newUser.UserId = resp.Id;
            await _repository.AddUserPassesAsync(newUser);//pass
            return newUser.UserId.ToString();
        }

        public void RegistrationPassCheck(RegisterRequest request)
        {
            if (!PasswordPreCheck(request.Password))
            {
                throw new ArgumentException("Password does not meet the criteria.");
            }
        }
        public bool PasswordPreCheck(string password)
        {
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{10,}$");
            //email validation
            return regex.IsMatch(password);
        }

        public async Task<int> SignInAsync(LoginRequestDTO login)
        {
            //var user = await _repository.GetUserByEmailAsync(login.Email);
            return 0;  
        }

        public IRepository Get_repository()
        {
            return _repository;
        }

          public int IsValidEmail(string email)
          {
            if (email.Length == 0 || email == null)
            {
                return -1;
            }

            int notFirstIndex = email.IndexOf('@');
            if (notFirstIndex < 0)
            {
                return -2;
            }

            int dotIndex = email.IndexOf('.', notFirstIndex);
            if (dotIndex <= notFirstIndex + 1)
            {
                return -3;
            }

            return 0;
          }

    }
}

