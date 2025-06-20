using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services.ApiServices;
using System.Security.Claims;

namespace eStoreClient.Pages.Member.Orders
{
    [Authorize(Roles = "Member")]
    public class DetailsModel : PageModel
    {
        private readonly OrderApiService _orderApiService;
        
        public DetailsModel(OrderApiService orderApiService)
        {
            _orderApiService = orderApiService;
        }
        
        public Order Order { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            int memberId = GetCurrentMemberId();
            if (memberId <= 0)
            {
                return RedirectToPage("/Account/Login");
            }
            
            Order = await _orderApiService.GetOrderByIdAsync(id);
            
            if (Order == null || Order.MemberId != memberId)
            {
                return NotFound();
            }
            
            OrderDetails = await _orderApiService.GetOrderDetailsAsync(id);
            
            CalculateTotals();
            
            return Page();
        }
        
        private void CalculateTotals()
        {
            Subtotal = 0;
            foreach (var item in OrderDetails)
            {
                decimal itemTotal = (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                decimal discountAmount = itemTotal * (decimal)(item.Discount ?? 0);
                Subtotal += (itemTotal - discountAmount);
            }
            
            Total = Subtotal + (Order.Freight ?? 0);
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