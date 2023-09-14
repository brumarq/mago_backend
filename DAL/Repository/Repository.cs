using System;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Model.Entities;

namespace DAL.Repository
{
	public class Repository<T> : IRepository<T> where T : BaseEntity
	{
	

        private readonly CustomDbContext _context;
        private readonly DbSet<T> _entities;



        public Repository(CustomDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }



        public async Task<T> CreateAsync(T entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }



        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entities.IgnoreAutoIncludes().ToListAsync();
        }



        public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }



        public async Task<bool?> UpdateAsync(T entity)
        {
            T? existingEntity = await _entities.FindAsync(entity.Id);



            if (entity == null)
                return false;



            _entities.Entry(existingEntity).CurrentValues.SetValues(entity);
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool?> DeleteAsync(int id)
        {
            var entity = await _entities.FindAsync(id);



            if (entity == null)
                return false;



            _entities.Remove(entity);



            return await _context.SaveChangesAsync() > 0;
        }
    }
}

