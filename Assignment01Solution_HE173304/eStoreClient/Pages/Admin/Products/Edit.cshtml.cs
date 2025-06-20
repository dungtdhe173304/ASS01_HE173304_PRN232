using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eStoreClient.Services.ApiServices;
using Repositories;

namespace eStoreClient.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        private readonly IRepositoryFactory _repositoryFactory; // Still needed for categories
        
        public EditModel(ProductApiService productApiService, IRepositoryFactory repositoryFactory)
        {
            _productApiService = productApiService;
            _repositoryFactory = repositoryFactory;
        }
        
        [BindProperty]
        public Product Product { get; set; } = null!;
        
        public SelectList? Categories { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productApiService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            Product = product;
            
            LoadCategories();
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }
            
            try
            {
                var result = await _productApiService.UpdateProductAsync(Product.ProductId, Product);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the product.");
                    LoadCategories();
                    return Page();
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                LoadCategories();
                return Page();
            }
        }
        
        private void LoadCategories()
        {
            var categoryRepository = _repositoryFactory.GetCategoryRepository();
            Categories = new SelectList(categoryRepository.GetAll(), "CategoryId", "CategoryName");
        }
    }
}