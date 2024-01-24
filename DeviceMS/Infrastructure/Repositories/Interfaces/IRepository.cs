using System.Linq.Expressions;
using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<bool> IsDatabaseConnected();
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync(int? pageNumber, int? pageSize);
    Task<IEnumerable<T>> GetCollectionByConditionAsync(Expression<Func<T, bool>> predicate, int? pageNumber = null, int? pageSize = null);
    Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<bool?> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}