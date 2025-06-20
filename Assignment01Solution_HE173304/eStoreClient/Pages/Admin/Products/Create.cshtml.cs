using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eStoreClient.Services.ApiServices;
using Repositories;

namespace eStoreClient.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        private readonly IRepositoryFactory _repositoryFactory; // Still needed for categories
        
        public CreateModel(ProductApiService productApiService, IRepositoryFactory repositoryFactory)
        {
            _productApiService = productApiService;
            _repositoryFactory = repositoryFactory;
        }
        
        public SelectList? Categories { get; set; }
        
        [BindProperty]
        public Product Product { get; set; } = new Product();
        
        public void OnGet()
        {
            var categoryRepository = _repositoryFactory.GetCategoryRepository();
            Categories = new SelectList(categoryRepository.GetAll(), "CategoryId", "CategoryName");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoryRepository = _repositoryFactory.GetCategoryRepository();
                Categories = new SelectList(categoryRepository.GetAll(), "CategoryId", "CategoryName");
                return Page();
            }
            
            // Check if product with same ID already exists
            if (await _productApiService.ExistsAsync(Product.ProductId))
            {
                ModelState.AddModelError(string.Empty, "A product with this ID already exists.");
                var categoryRepository = _repositoryFactory.GetCategoryRepository();
                Categories = new SelectList(categoryRepository.GetAll(), "CategoryId", "CategoryName");
                return Page();
            }
            
            var result = await _productApiService.CreateProductAsync(Product);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                var categoryRepository = _repositoryFactory.GetCategoryRepository();
                Categories = new SelectList(categoryRepository.GetAll(), "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}