using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using System.Security.Claims;

namespace eStoreClient.Pages.Member
{
    [Authorize(Roles = "Member")]
    public class IndexModel : PageModel
    {
        private readonly IRepositoryFactory _repositoryFactory;
        
        public IndexModel(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }
        
        public BusinessObject.Models.Member CurrentMember { get; set; } = new BusinessObject.Models.Member();
        public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();
        
        public IActionResult OnGet()
        {
            int memberId = GetCurrentMemberId();
            if (memberId <= 0)
            {
                return RedirectToPage("/Account/Login");
            }
            
            var memberRepository = _repositoryFactory.GetMemberRepository();
            var orderRepository = _repositoryFactory.GetOrderRepository();
            
            CurrentMember = memberRepository.GetById(memberId) ?? new BusinessObject.Models.Member();
            
            if (CurrentMember == null)
            {
                return NotFound();
            }
            
            // Get recent orders
            RecentOrders = orderRepository.GetOrdersByMember(memberId)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();
            
            return Page();
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