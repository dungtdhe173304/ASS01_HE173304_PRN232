using BusinessObject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Repositories;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace eStoreClient.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConfiguration _configuration;

        public LoginModel(IRepositoryFactory repositoryFactory, IConfiguration configuration)
        {
            _repositoryFactory = repositoryFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public LoginInputModel LoginInput { get; set; } = new LoginInputModel();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var memberRepository = _repositoryFactory.GetMemberRepository();
            
            // Check if admin login
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];
            
            if (LoginInput.Email == adminEmail && LoginInput.Password == adminPassword)
            {
                // Create claims for admin
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Email, adminEmail),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                
                await SignInUserAsync(claims);
                return RedirectToPage("/Admin/Index");
            }
            
            // Check regular member login
            var member = memberRepository.GetByEmail(LoginInput.Email);
            if (member == null || member.Password != LoginInput.Password)
            {
                ErrorMessage = "Invalid email or password";
                return Page();
            }
            
            // Create claims for member
            var memberClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Email ?? ""),
                new Claim(ClaimTypes.Email, member.Email ?? ""),
                new Claim(ClaimTypes.Role, "Member"),
                new Claim("MemberId", member.MemberId.ToString())
            };
            
            await SignInUserAsync(memberClaims);
            return RedirectToPage("/Member/Index");
        }
        
        private async Task SignInUserAsync(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}