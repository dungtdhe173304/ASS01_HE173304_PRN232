using BusinessObject.Models;
using DataAccessObject;

namespace Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly IProductDAO _productDAO;

    public ProductRepository(IProductDAO productDAO) : base(productDAO)
    {
        _productDAO = productDAO;
    }

    public IEnumerable<Product> GetProductsByCategory(int categoryId)
    {
        return _productDAO.GetProductsByCategory(categoryId);
    }

    public IEnumerable<Product> GetProductsInStock()
    {
        return _productDAO.GetProductsInStock();
    }

    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        return _productDAO.SearchProducts(searchTerm);
    }
}