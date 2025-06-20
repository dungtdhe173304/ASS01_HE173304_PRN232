using BusinessObject.Models;

namespace DataAccessObject;

public interface ICategoryDAO : IGenericDAO<Category>
{
    IEnumerable<Category> GetCategoriesWithProducts();
}