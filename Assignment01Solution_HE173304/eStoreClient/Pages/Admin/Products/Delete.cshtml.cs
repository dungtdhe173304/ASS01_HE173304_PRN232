using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services.ApiServices;

namespace eStoreClient.Pages.Admin.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        
        public DeleteModel(ProductApiService productApiService)
        {
            _productApiService = productApiService;
        }
        
        [BindProperty]
        public Product Product { get; set; } = null!;
        
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Try to delete the product through the API
                var (success, errorMessage) = await _productApiService.DeleteProductAsync(Product.ProductId);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? 
                        "This product cannot be deleted because it is referenced by existing orders.");
                    return Page();
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
                return Page();
            }
        }
    }
}