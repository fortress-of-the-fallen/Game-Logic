using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Application.Infra.DataAccess.Repository;
using Game.Application.Interface.DataAccess.Repository;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Game.Application.Infra.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;
        public DbContext DbContext { get; }

        public UnitOfWork()
        {
            DbContext = Dependencies.Db;
            _repositories = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(Repository<TEntity>);

            if (!_repositories.TryGetValue(type, out var value))
            {
                value = new Repository<TEntity>(DbContext);
                _repositories[type] = value;
            }

            return (IRepository<TEntity>)value;
        }

        public Task<int> SaveChanges()
        {
            return DbContext.SaveChangesAsync();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                    _repositories.Clear();
                }
                _disposed = true;
            }
        }
    }
}