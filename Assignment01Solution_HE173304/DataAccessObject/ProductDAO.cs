using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject;

public class ProductDAO : GenericDAO<Product>, IProductDAO
{
    public IEnumerable<Product> GetProductsByCategory(int categoryId)
    {
        return _context.Products.Where(p => p.CategoryId == categoryId).ToList();
    }

    public IEnumerable<Product> GetProductsInStock()
    {
        return _context.Products.Where(p => p.UnitsInStock > 0).ToList();
    }

    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        return _context.Products
            .Where(p => p.ProductName != null && p.ProductName.Contains(searchTerm))
            .ToList();
    }
    
    // Override GetById for Product entity
    public new Product? GetById(object id)
    {
        if (id is int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.OrderDetails)
                .FirstOrDefault(p => p.ProductId == productId);
        }
        return null;
    }
}