using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services.ApiServices;

namespace eStoreClient.Pages.Admin.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        
        public DetailsModel(ProductApiService productApiService)
        {
            _productApiService = productApiService;
        }
        
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
    }
}