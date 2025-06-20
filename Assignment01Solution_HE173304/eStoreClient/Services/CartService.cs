using System.Text.Json;

namespace eStoreClient.Services;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "ShoppingCart";

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<CartItem> GetCart()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        string? cartJson = session?.GetString(CartSessionKey);
        
        if (string.IsNullOrEmpty(cartJson))
            return new List<CartItem>();
            
        return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
    }

    public void SaveCart(List<CartItem> cart)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        string cartJson = JsonSerializer.Serialize(cart);
        session?.SetString(CartSessionKey, cartJson);
    }

    public void AddItem(int productId, string productName, decimal price, int quantity = 1)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                UnitPrice = price,
                Quantity = quantity
            });
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            if (quantity > 0)
                existingItem.Quantity = quantity;
            else
                cart.Remove(existingItem);
        }

        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
            cart.Remove(existingItem);

        SaveCart(cart);
    }

    public void ClearCart()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(CartSessionKey);
    }

    public decimal GetTotalPrice() => GetCart().Sum(item => item.Total);
    
    public int GetTotalItems() => GetCart().Sum(item => item.Quantity);
}