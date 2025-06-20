using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject;

public class CategoryDAO : GenericDAO<Category>, ICategoryDAO
{
    public IEnumerable<Category> GetCategoriesWithProducts()
    {
        return _context.Categories.Include(c => c.Products).ToList();
    }
    
    // Override GetById for Category entity
    public new Category? GetById(object id)
    {
        if (id is int categoryId)
        {
            return _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryId == categoryId);
        }
        return null;
    }
}