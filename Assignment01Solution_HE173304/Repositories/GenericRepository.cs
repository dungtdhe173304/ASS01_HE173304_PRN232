using System.Linq.Expressions;
using DataAccessObject;

namespace Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly IGenericDAO<T> _dao;

    public GenericRepository(IGenericDAO<T> dao)
    {
        _dao = dao;
    }

    public virtual IEnumerable<T> GetAll()
    {
        return _dao.GetAll();
    }

    public virtual T? GetById(object id)
    {
        return _dao.GetById(id);
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return _dao.Find(expression);
    }

    public virtual void Add(T entity)
    {
        _dao.Add(entity);
    }

    public virtual void Update(T entity)
    {
        _dao.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dao.Remove(entity);
    }

    public virtual void Remove(object id)
    {
        _dao.Remove(id);
    }

    public virtual bool Exists(object id)
    {
        return _dao.Exists(id);
    }
}