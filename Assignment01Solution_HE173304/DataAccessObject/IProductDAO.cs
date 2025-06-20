using BusinessObject.Models;

namespace DataAccessObject;

public interface IProductDAO : IGenericDAO<Product>
{
    IEnumerable<Product> GetProductsByCategory(int categoryId);
    IEnumerable<Product> GetProductsInStock();
    IEnumerable<Product> SearchProducts(string searchTerm);
}