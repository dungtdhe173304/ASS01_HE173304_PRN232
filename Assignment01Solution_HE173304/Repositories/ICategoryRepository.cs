using BusinessObject.Models;

namespace Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    IEnumerable<Category> GetCategoriesWithProducts();
}