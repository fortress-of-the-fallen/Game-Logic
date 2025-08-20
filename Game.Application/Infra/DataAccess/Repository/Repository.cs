using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Game.Application.Interface.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace Game.Application.Infra.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> DbSet;

        public Repository(DbContext dbContext)
        {
            DbSet = dbContext.Set<T>();
        }

        public async Task Add(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public async Task Add(params T[] entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public async Task Add(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public async Task<bool> Any(Expression<Func<T, bool>> expression)
        {
            return await DbSet.AnyAsync(expression);
        }

        public async Task Delete(object[] keyValues)
        {
            var entity = await DbSet.FindAsync(keyValues);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public async Task Delete(object id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public async Task Delete(T entity)
        {
            DbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task Delete(params T[] entities)
        {
            DbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task Delete(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<IQueryable<T>> QueryAll(bool disableTracking = true)
        {
            if (disableTracking)
            {
                return await Task.FromResult(DbSet.AsNoTracking());
            }
            return await Task.FromResult(DbSet);
        }

        public async Task<IQueryable<T>> QueryCondition(Expression<Func<T, bool>> expression, bool disableTracking = true)
        {
            if (disableTracking)
            {
                return await Task.FromResult(DbSet.AsNoTracking().Where(expression));
            }
            return await Task.FromResult(DbSet.Where(expression));
        }

        public async Task<IQueryable<TType>> Select<TType>(Expression<Func<T, TType>> select)
        {
            return await Task.FromResult(DbSet.Select(select));
        }

        public async Task<T> Single(Expression<Func<T, bool>> predicate = null, bool disableTracking = true)
        {
            IQueryable<T> query = disableTracking ? DbSet.AsNoTracking() : DbSet;

            if (disableTracking)
            {
                if (predicate == null)
                    return await query.SingleOrDefaultAsync();
                else
                    return await query.SingleOrDefaultAsync(predicate);
            }
            
            if (predicate == null)
                return await query.SingleOrDefaultAsync();
            else
                return await query.SingleOrDefaultAsync(predicate);
        }

        public async Task Update(T entity)
        {
            DbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task Update(params T[] entities)
        {
            DbSet.UpdateRange(entities);
            await Task.CompletedTask;
        }

        public async Task Update(IEnumerable<T> entities)
        {
            DbSet.UpdateRange(entities);
            await Task.CompletedTask;
        }
    }
}