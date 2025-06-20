using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services;

namespace eStoreClient.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly CartService _cartService;

        public IndexModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }

        public void OnGet()
        {
            CartItems = _cartService.GetCart();
            TotalPrice = _cartService.GetTotalPrice();
        }

        public IActionResult OnPostUpdateQuantity(int productId, int quantity)
        {
            if (quantity > 0)
            {
                _cartService.UpdateQuantity(productId, quantity);
                TempData["SuccessMessage"] = "Cart updated successfully!";
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int productId)
        {
            _cartService.RemoveItem(productId);
            TempData["SuccessMessage"] = "Item removed from cart!";
            return RedirectToPage();
        }

        public IActionResult OnPostClear()
        {
            _cartService.ClearCart();
            TempData["SuccessMessage"] = "Shopping cart cleared!";
            return RedirectToPage();
        }
    }
}