using BusinessObject.Models;
using DataAccessObject;

namespace Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly ICategoryDAO _categoryDAO;

    public CategoryRepository(ICategoryDAO categoryDAO) : base(categoryDAO)
    {
        _categoryDAO = categoryDAO;
    }

    public IEnumerable<Category> GetCategoriesWithProducts()
    {
        return _categoryDAO.GetCategoriesWithProducts();
    }
}