using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eStoreClient.Services.ApiServices;
using System.Linq;
using Repositories;

namespace eStoreClient.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IRepositoryFactory _repositoryFactory; // Still needed for Members and Categories
        private readonly ProductApiService _productApiService;
        private readonly OrderApiService _orderApiService;
        
        public IndexModel(
            IRepositoryFactory repositoryFactory,
            ProductApiService productApiService,
            OrderApiService orderApiService)
        {
            _repositoryFactory = repositoryFactory;
            _productApiService = productApiService;
            _orderApiService = orderApiService;
        }
        
        public int MemberCount { get; set; }
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int OrderCount { get; set; }
        
        public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();
        public IEnumerable<Product> LowStockProducts { get; set; } = new List<Product>();
        
        public async Task OnGetAsync()
        {
            var memberRepository = _repositoryFactory.GetMemberRepository();
            var categoryRepository = _repositoryFactory.GetCategoryRepository();
            
            // Get counts from repositories and API
            MemberCount = memberRepository.GetAll().Count();
            CategoryCount = categoryRepository.GetAll().Count();
            
            var allProducts = await _productApiService.GetAllProductsAsync();
            ProductCount = allProducts.Count();
            
            var allOrders = await _orderApiService.GetAllOrdersAsync();
            OrderCount = allOrders.Count();
            
            // Get 5 most recent orders
            var today = DateOnly.FromDateTime(DateTime.Today);
            var thirtyDaysAgo = today.AddDays(-30);
            
            RecentOrders = (await _orderApiService.GetOrdersByDateRangeAsync(thirtyDaysAgo, today))
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();
                
            // Get low stock products (less than 10 units)
            LowStockProducts = await _productApiService.GetLowStockProductsAsync();
        }
    }
}