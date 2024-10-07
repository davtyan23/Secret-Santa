using Business.DTOs.Request;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public Task<int> SignInAsync(LoginRequestDTO login)
        {
            return Task.FromResult(0);  
        }


        Task<int> IAuthService.RegisterUserAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}

