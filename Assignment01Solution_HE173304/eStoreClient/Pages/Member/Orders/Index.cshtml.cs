using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services.ApiServices;
using System.Security.Claims;

namespace eStoreClient.Pages.Member.Orders
{
    [Authorize(Roles = "Member")]
    public class IndexModel : PageModel
    {
        private readonly OrderApiService _orderApiService;
        
        public IndexModel(OrderApiService orderApiService)
        {
            _orderApiService = orderApiService;
        }
        
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        
        public async Task<IActionResult> OnGetAsync()
        {
            int memberId = GetCurrentMemberId();
            if (memberId <= 0)
            {
                return RedirectToPage("/Account/Login");
            }
            
            Orders = await _orderApiService.GetOrdersByMemberAsync(memberId);
            
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