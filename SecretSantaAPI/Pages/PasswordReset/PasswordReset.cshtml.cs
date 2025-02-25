using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecretSantaAPI.Pages.PasswordReset
{
    public class PasswordResetModel : PageModel
    {
        private readonly IRepository _repository;
        public PasswordResetModel(IRepository repository)
        {
            _repository = repository;
        }
        public string EmailError { get; set; } = string.Empty;
        public void OnGet()
        {
            string GenerateCode(string code) 
            {
                Random random = new Random();
                code = random.Next(100000,999999).ToString();
                return code;
            }   
        }

       
    }
}
