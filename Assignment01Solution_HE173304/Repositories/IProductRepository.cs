using BusinessObject.Models;

namespace Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    IEnumerable<Product> GetProductsByCategory(int categoryId);
    IEnumerable<Product> GetProductsInStock();
    IEnumerable<Product> SearchProducts(string searchTerm);
}