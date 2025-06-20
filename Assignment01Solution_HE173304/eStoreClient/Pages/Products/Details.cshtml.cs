using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services;
using eStoreClient.Services.ApiServices;

namespace eStoreClient.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        private readonly CartService _cartService;

        public DetailsModel(ProductApiService productApiService, CartService cartService)
        {
            _productApiService = productApiService;
            _cartService = cartService;
        }

        public Product Product { get; set; } = null!;

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productApiService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            var product = await _productApiService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (Quantity <= 0 || Quantity > product.UnitsInStock)
            {
                ModelState.AddModelError("Quantity", "Invalid quantity selected.");
                Product = product;
                return Page();
            }

            _cartService.AddItem(
                product.ProductId,
                product.ProductName ?? "Unknown Product",
                product.UnitPrice ?? 0m,
                Quantity);

            TempData["SuccessMessage"] = $"{Quantity} {product.ProductName} added to your cart!";
            return RedirectToPage("./Index");
        }
    }
}