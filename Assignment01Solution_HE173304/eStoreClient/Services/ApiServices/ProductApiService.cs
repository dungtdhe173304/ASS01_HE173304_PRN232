using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace eStoreClient.Services.ApiServices;

public class ProductApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IRepositoryFactory _repositoryFactory;

    public ProductApiService(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        IRepositoryFactory repositoryFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7000/api";
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _repositoryFactory = repositoryFactory;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try 
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>(_jsonOptions) ?? Enumerable.Empty<Product>();
            }

            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetAll();
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetAll();
        }
    }

    public async Task<IEnumerable<Product>> GetProductsInStockAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/instock");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>(_jsonOptions) ?? Enumerable.Empty<Product>();
            }

            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetProductsInStock();
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetProductsInStock();
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetById(id);
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetById(id);
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        try 
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/category/{categoryId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>(_jsonOptions) ?? Enumerable.Empty<Product>();
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetProductsByCategory(categoryId);
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetProductsByCategory(categoryId);
        }
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string term)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/search?term={Uri.EscapeDataString(term)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>(_jsonOptions) ?? Enumerable.Empty<Product>();
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.SearchProducts(term);
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.SearchProducts(term);
        }
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/lowstock");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>(_jsonOptions) ?? Enumerable.Empty<Product>();
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetAll()
                .Where(p => p.UnitsInStock < 10)
                .OrderBy(p => p.UnitsInStock);
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.GetAll()
                .Where(p => p.UnitsInStock < 10)
                .OrderBy(p => p.UnitsInStock);
        }
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        try
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Product", content);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            
            if (productRepository.Exists(product.ProductId))
            {
                return false;
            }
            
            productRepository.Add(product);
            return true;
        }
        catch
        {
            // Fallback to repository when API call fails
            try
            {
                var productRepository = _repositoryFactory.GetProductRepository();
                
                if (productRepository.Exists(product.ProductId))
                {
                    return false;
                }
                
                productRepository.Add(product);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<bool> UpdateProductAsync(int id, Product product)
    {
        try
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Product/{id}", content);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            
            // Don't check existence with Exists() as it would load and track the entity
            // Just attempt the update directly
            try
            {
                productRepository.Update(product);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // This would occur if the product doesn't exist
                return false;
            }
        }
        catch
        {
            // Fallback to repository when API call fails
            try
            {
                var productRepository = _repositoryFactory.GetProductRepository();
                
                // Don't check existence with Exists() as it would load and track the entity
                // Just attempt the update directly
                try
                {
                    productRepository.Update(product);
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // This would occur if the product doesn't exist
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Product/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, errorContent);
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
            
            if (!productRepository.Exists(id))
            {
                return (false, "Product not found");
            }
            
            // Check if product is used in any orders
            var orderDetails = orderDetailRepository.GetOrderDetailsByProduct(id);
            
            if (orderDetails.Any())
            {
                return (false, "This product cannot be deleted because it is referenced by existing orders");
            }
            
            productRepository.Remove(id);
            return (true, null);
        }
        catch (Exception ex)
        {
            // Fallback to repository when API call fails
            try
            {
                var productRepository = _repositoryFactory.GetProductRepository();
                var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
                
                if (!productRepository.Exists(id))
                {
                    return (false, "Product not found");
                }
                
                // Check if product is used in any orders
                var orderDetails = orderDetailRepository.GetOrderDetailsByProduct(id);
                
                if (orderDetails.Any())
                {
                    return (false, "This product cannot be deleted because it is referenced by existing orders");
                }
                
                productRepository.Remove(id);
                return (true, null);
            }
            catch (Exception repoEx)
            {
                return (false, $"An error occurred while deleting the product: {repoEx.Message}");
            }
        }
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            // Fallback to direct repository access
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.Exists(id);
        }
        catch
        {
            // Fallback to repository when API call fails
            var productRepository = _repositoryFactory.GetProductRepository();
            return productRepository.Exists(id);
        }
    }
}