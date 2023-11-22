using System.Linq.Expressions;
using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<bool?> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}