using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services;
using eStoreClient.Services.ApiServices;
using System.Security.Claims;

namespace eStoreClient.Pages.Cart
{
    [Authorize(Roles = "Member")]
    public class CheckoutModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        private readonly OrderApiService _orderApiService;
        private readonly CartService _cartService;

        public CheckoutModel(
            ProductApiService productApiService,
            OrderApiService orderApiService,
            CartService cartService)
        {
            _productApiService = productApiService;
            _orderApiService = orderApiService;
            _cartService = cartService;
        }

        [BindProperty]
        public Order Order { get; set; } = new Order();

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal SubTotal { get; set; }
        public decimal TotalPrice { get; set; }

        public IActionResult OnGet()
        {
            CartItems = _cartService.GetCart();
            
            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart/Index");
            }

            // Default shipping cost calculation
            decimal shippingCost = CartItems.Sum(x => x.Quantity) * 0.5m;
            // Minimum shipping cost
            shippingCost = Math.Max(5, shippingCost);
            // Maximum shipping cost
            shippingCost = Math.Min(20, shippingCost);

            SubTotal = _cartService.GetTotalPrice();
            Order.Freight = shippingCost;
            TotalPrice = SubTotal + shippingCost;
            
            // Set the required date to a week from today by default
            Order.RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return OnGet();
            }

            CartItems = _cartService.GetCart();
            
            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart/Index");
            }

            int memberId = GetCurrentMemberId();
            
            if (memberId <= 0)
            {
                return RedirectToPage("/Account/Login");
            }

            // Set order properties
            Order.MemberId = memberId;
            Order.OrderDate = DateOnly.FromDateTime(DateTime.Today);

            // Create the order through the API
            var createdOrder = await _orderApiService.CreateOrderAsync(Order);
            
            if (createdOrder == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to create order.");
                return OnGet();
            }

            // Create order details
            bool orderDetailsSuccess = true;
            foreach (var item in CartItems)
            {
                // Verify product availability through API
                var product = await _productApiService.GetProductByIdAsync(item.ProductId);
                if (product == null || product.UnitsInStock < item.Quantity)
                {
                    ModelState.AddModelError(string.Empty, 
                        $"Product {item.ProductName} is no longer available in the requested quantity.");
                    return OnGet();
                }

                var orderDetail = new OrderDetail
                {
                    OrderId = createdOrder.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = 0
                };

                // Add order detail through API
                var detailResult = await _orderApiService.AddOrderDetailAsync(createdOrder.OrderId, orderDetail);
                if (!detailResult)
                {
                    orderDetailsSuccess = false;
                    break;
                }
            }

            if (!orderDetailsSuccess)
            {
                // Handle error - potentially try to delete the order
                await _orderApiService.DeleteOrderAsync(createdOrder.OrderId);
                ModelState.AddModelError(string.Empty, "Failed to create order details.");
                return OnGet();
            }

            // Clear the cart
            _cartService.ClearCart();

            // Redirect to the order confirmation page
            return RedirectToPage("/Member/Orders/Details", new { id = createdOrder.OrderId });
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