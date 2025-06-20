using System.Linq.Expressions;

namespace Repositories;

public interface IGenericRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(object id);
    IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    void Remove(object id);
    bool Exists(object id);
}