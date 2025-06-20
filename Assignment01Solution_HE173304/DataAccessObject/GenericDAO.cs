using System.Linq.Expressions;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject;

public class GenericDAO<T> : IGenericDAO<T> where T : class
{
    protected readonly StoreDbContext _context;
    
    public GenericDAO()
    {
        _context = new StoreDbContext();
    }
    
    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? GetById(object id)
    {
        return _context.Set<T>().Find(id);
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression).ToList();
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
    }

    public void Remove(object id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            Remove(entity);
        }
    }

    public bool Exists(object id)
    {
        return GetById(id) != null;
    }

    public bool ExistsWithoutTracking(object id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            // Detach the entity from the context
            _context.Entry(entity).State = EntityState.Detached;
            return true;
        }
        return false;
    }
}