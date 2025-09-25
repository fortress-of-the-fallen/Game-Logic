using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Game.Application.Interface.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T> Single(Expression<Func<T, bool>> predicate = null, bool disableTracking = true);

        Task<IQueryable<T>> QueryAll(bool disableTracking = true);

        Task<IQueryable<T>> QueryCondition(Expression<Func<T, bool>> expression, bool disableTracking = true);

        Task<bool> Any(Expression<Func<T, bool>> expression);

        Task<IQueryable<TType>> Select<TType>(Expression<Func<T, TType>> select);

        Task Add(T entity);

        Task Add(params T[] entities);

        Task Add(IEnumerable<T> entities);

        Task Update(T entity);

        Task Update(params T[] entities);

        Task Update(IEnumerable<T> entities);

        Task Delete(object[] keyValues);

        Task Delete(object id);

        Task Delete(T entity);

        Task Delete(params T[] entities);

        Task Delete(IEnumerable<T> entities);
    }
}