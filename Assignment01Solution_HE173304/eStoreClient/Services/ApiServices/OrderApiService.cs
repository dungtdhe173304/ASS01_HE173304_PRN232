using BusinessObject.Models;
using Repositories;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace eStoreClient.Services.ApiServices;

public class OrderApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IRepositoryFactory _repositoryFactory;

    public OrderApiService(
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

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Order");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Order>>(_jsonOptions) ?? Enumerable.Empty<Order>();
        }
        catch
        {
            // Fallback to repository when API call fails
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetAll();
        }
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Order/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>(_jsonOptions);
            }
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetById(id);
        }
        catch
        {
            // Fallback to repository when API call fails
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetById(id);
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersByMemberAsync(int memberId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Order/member/{memberId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Order>>(_jsonOptions) ?? Enumerable.Empty<Order>();
            }
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetOrdersByMember(memberId);
        }
        catch
        {
            // Fallback to repository when API call fails
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetOrdersByMember(memberId);
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Order/daterange?startDate={startDate}&endDate={endDate}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Order>>(_jsonOptions) ?? Enumerable.Empty<Order>();
            }
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetOrdersByDateRange(startDate, endDate);
        }
        catch
        {
            // Fallback to repository when API call fails
            var orderRepository = _repositoryFactory.GetOrderRepository();
            return orderRepository.GetOrdersByDateRange(startDate, endDate);
        }
    }

    public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync(int orderId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Order/{orderId}/details");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<OrderDetail>>(_jsonOptions) ?? Enumerable.Empty<OrderDetail>();
            }
            // If API returns non-success, fall back to repository
            var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
            return orderDetailRepository.GetOrderDetailsByOrder(orderId);
        }
        catch
        {
            // Fallback to repository when API call fails
            var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
            return orderDetailRepository.GetOrderDetailsByOrder(orderId);
        }
    }

    public async Task<Order?> CreateOrderAsync(Order order)
    {
        try
        {
            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Order", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>(_jsonOptions);
            }
            
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            
            // Set the order date if not provided
            if (order.OrderDate == null)
            {
                order.OrderDate = DateOnly.FromDateTime(DateTime.Today);
            }
            
            // Get the next available order ID if not provided
            if (order.OrderId <= 0)
            {
                int nextOrderId = orderRepository.GetAll().Any() 
                    ? orderRepository.GetAll().Max(o => o.OrderId) + 1 
                    : 1;
                order.OrderId = nextOrderId;
            }
            
            orderRepository.Add(order);
            return order;
        }
        catch
        {
            // Fallback to repository when API call fails
            var orderRepository = _repositoryFactory.GetOrderRepository();
            
            // Set the order date if not provided
            if (order.OrderDate == null)
            {
                order.OrderDate = DateOnly.FromDateTime(DateTime.Today);
            }
            
            // Get the next available order ID if not provided
            if (order.OrderId <= 0)
            {
                int nextOrderId = orderRepository.GetAll().Any() 
                    ? orderRepository.GetAll().Max(o => o.OrderId) + 1 
                    : 1;
                order.OrderId = nextOrderId;
            }
            
            orderRepository.Add(order);
            return order;
        }
    }

    public async Task<bool> AddOrderDetailAsync(int orderId, OrderDetail orderDetail)
    {
        try
        {
            var json = JsonSerializer.Serialize(orderDetail);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Order/{orderId}/details", content);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            if (!orderRepository.Exists(orderId))
            {
                return false;
            }
            
            var productRepository = _repositoryFactory.GetProductRepository();
            var product = productRepository.GetById(orderDetail.ProductId);
            
            if (product == null)
            {
                return false;
            }
            
            // Check if there's enough stock
            if (product.UnitsInStock < orderDetail.Quantity)
            {
                return false;
            }
            
            var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
            orderDetailRepository.Add(orderDetail);
            
            // Update product stock
            product.UnitsInStock -= orderDetail.Quantity;
            productRepository.Update(product);
            
            return true;
        }
        catch
        {
            // Fallback to repository when API call fails
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                if (!orderRepository.Exists(orderId))
                {
                    return false;
                }
                
                var productRepository = _repositoryFactory.GetProductRepository();
                var product = productRepository.GetById(orderDetail.ProductId);
                
                if (product == null)
                {
                    return false;
                }
                
                // Check if there's enough stock
                if (product.UnitsInStock < orderDetail.Quantity)
                {
                    return false;
                }
                
                var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
                orderDetailRepository.Add(orderDetail);
                
                // Update product stock
                product.UnitsInStock -= orderDetail.Quantity;
                productRepository.Update(product);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<bool> UpdateOrderAsync(int id, Order order)
    {
        try
        {
            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Order/{id}", content);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            
            if (!orderRepository.Exists(id))
            {
                return false;
            }
            
            orderRepository.Update(order);
            return true;
        }
        catch
        {
            // Fallback to repository when API call fails
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                
                if (!orderRepository.Exists(id))
                {
                    return false;
                }
                
                orderRepository.Update(order);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Order/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            // If API returns non-success, fall back to repository
            var orderRepository = _repositoryFactory.GetOrderRepository();
            
            if (!orderRepository.Exists(id))
            {
                return false;
            }
            
            var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
            var details = orderDetailRepository.GetOrderDetailsByOrder(id).ToList();
            
            // Return products to inventory
            var productRepository = _repositoryFactory.GetProductRepository();
            foreach (var detail in details)
            {
                var product = productRepository.GetById(detail.ProductId);
                if (product != null)
                {
                    product.UnitsInStock += detail.Quantity;
                    productRepository.Update(product);
                }
            }
            
            // Remove order (cascade will handle order details)
            orderRepository.Remove(id);
            return true;
        }
        catch
        {
            // Fallback to repository when API call fails
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                
                if (!orderRepository.Exists(id))
                {
                    return false;
                }
                
                var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
                var details = orderDetailRepository.GetOrderDetailsByOrder(id).ToList();
                
                // Return products to inventory
                var productRepository = _repositoryFactory.GetProductRepository();
                foreach (var detail in details)
                {
                    var product = productRepository.GetById(detail.ProductId);
                    if (product != null)
                    {
                        product.UnitsInStock += detail.Quantity;
                        productRepository.Update(product);
                    }
                }
                
                // Remove order (cascade will handle order details)
                orderRepository.Remove(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}