using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using eStoreClient.Services;

namespace eStoreClient.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly CartService _cartService;

        public IndexModel(IRepositoryFactory repositoryFactory, CartService cartService)
        {
            _repositoryFactory = repositoryFactory;
            _cartService = cartService;
        }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public SelectList Categories { get; set; } = null!;

        public async Task OnGetAsync()
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var categoryRepository = _repositoryFactory.GetCategoryRepository();
            
            // Load all categories for the dropdown
            var categories = categoryRepository.GetAll().ToList();
            Categories = new SelectList(categories, "CategoryId", "CategoryName");
            
            // Get products based on search and filter criteria
            IEnumerable<Product> products = productRepository.GetProductsInStock();
            
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == CategoryId.Value);
            }
            
            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(p => 
                    p.ProductName != null && 
                    p.ProductName.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
            }
            
            Products = products;
        }

        public IActionResult OnPostAddToCart(int id)
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var product = productRepository.GetById(id);
            
            if (product != null && product.UnitsInStock > 0)
            {
                _cartService.AddItem(
                    product.ProductId, 
                    product.ProductName ?? "Unknown Product", 
                    product.UnitPrice ?? 0m);
                
                TempData["SuccessMessage"] = $"{product.ProductName} has been added to your cart!";
            }
            
            return RedirectToPage();
        }
    }
}