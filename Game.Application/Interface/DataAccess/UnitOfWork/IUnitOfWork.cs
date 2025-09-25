using System;
using System.Threading.Tasks;
using Game.Application.Interface.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace Game.Application.Interface.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext DbContext { get; }

        IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

        Task<int> SaveChanges();
    }
}