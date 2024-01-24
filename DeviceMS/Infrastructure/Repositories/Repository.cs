using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Data.Context;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DevicesDbContext _context;
    private readonly DbSet<T> _entities;

    public Repository(DevicesDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public async Task<bool> IsDatabaseConnected()
    {
        return await _context.Database.CanConnectAsync();
    }
    
    public async Task<T> CreateAsync(T entity)
    {
        _entities.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            return await _entities.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();
        }

        return await _entities.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetCollectionByConditionAsync(Expression<Func<T, bool>> predicate, int? pageNumber, int? pageSize)
    {
        var query = _entities.Where(predicate);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool?> UpdateAsync(T entity)
    {
        var existingEntity = await _entities.FindAsync(entity.Id);

        if (existingEntity == null)
            return null;

        _entities.Entry(existingEntity).CurrentValues.SetValues(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _entities.FindAsync(id);

        if (entity == null)
            return false;

        _entities.Remove(entity);

        return await _context.SaveChangesAsync() > 0;
    }
}