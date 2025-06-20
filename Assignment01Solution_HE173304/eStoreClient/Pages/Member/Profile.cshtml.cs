using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using System.Security.Claims;

namespace eStoreClient.Pages.Member
{
    [Authorize(Roles = "Member")]
    public class ProfileModel : PageModel
    {
        private readonly IRepositoryFactory _repositoryFactory;
        
        public ProfileModel(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }
        
        [BindProperty]
        public BusinessObject.Models.Member MemberProfile { get; set; } = new BusinessObject.Models.Member();
        
        public IActionResult OnGet()
        {
            int memberId = GetCurrentMemberId();
            if (memberId <= 0)
            {
                return RedirectToPage("/Account/Login");
            }
            
            var memberRepository = _repositoryFactory.GetMemberRepository();
            MemberProfile = memberRepository.GetById(memberId) ?? new BusinessObject.Models.Member();
            
            if (MemberProfile == null)
            {
                return NotFound();
            }
            
            return Page();
        }
        
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var memberRepository = _repositoryFactory.GetMemberRepository();
            memberRepository.Update(MemberProfile);
            
            TempData["SuccessMessage"] = "Profile updated successfully";
            return RedirectToPage();
        }
        
        private int GetCurrentMemberId()
        {
            var memberIdClaim = User.FindFirst("MemberId");
            if (memberIdClaim != null && int.TryParse(memberIdClaim.Value, out int memberId))
            {
                return memberId;
            }
            return -1;
        }
    }
}