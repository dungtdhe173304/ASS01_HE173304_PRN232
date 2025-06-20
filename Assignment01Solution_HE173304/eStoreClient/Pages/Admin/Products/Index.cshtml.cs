using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eStoreClient.Services.ApiServices;
using Repositories;

namespace eStoreClient.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductApiService _productApiService;
        private readonly IRepositoryFactory _repositoryFactory; // Still needed for categories

        public IndexModel(ProductApiService productApiService, IRepositoryFactory repositoryFactory)
        {
            _productApiService = productApiService;
            _repositoryFactory = repositoryFactory;
        }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public SelectList Categories { get; set; } = null!;

        public async Task OnGetAsync()
        {
            var categoryRepository = _repositoryFactory.GetCategoryRepository();
            
            // Load all categories for the dropdown
            var categories = categoryRepository.GetAll().ToList();
            Categories = new SelectList(categories, "CategoryId", "CategoryName");
            
            // Get products based on search and filter criteria
            IEnumerable<Product> products;
            
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                products = await _productApiService.GetProductsByCategoryAsync(CategoryId.Value);
            }
            else
            {
                products = await _productApiService.GetAllProductsAsync();
            }
            
            if (!string.IsNullOrEmpty(SearchString))
            {
                products = await _productApiService.SearchProductsAsync(SearchString);
            }
            
            Products = products;
        }
    }
}