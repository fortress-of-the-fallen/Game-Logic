using System;
using System.Collections.Generic;
using Game.Application.Infra.DataAccess;
using Game.Application.Infra.DataAccess.UnitOfWork;
using Game.Application.Infra.Http;
using Game.Application.Infra.Logging;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Base.Locator;
using Microsoft.EntityFrameworkCore;

namespace Game.Application
{
    public static class Dependencies
    {
        private static readonly ServiceLocator singleTonLocator = new ServiceLocator(scope: Scope.Singleton);
        private static readonly Dictionary<Dictionary<Type, Type>, Scope> registeredServices
            = new Dictionary<Dictionary<Type, Type>, Scope>()
            {
                { new Dictionary<Type, Type> { { typeof(ILogger<>), typeof(Logger<>) } }, Scope.Singleton },
                { new Dictionary<Type, Type> { { typeof(IRestfulService), typeof(RestfulService) } }, Scope.Singleton },
                { new Dictionary<Type, Type> { { typeof(IContextAccessor), typeof(ContextAccessor) } }, Scope.Singleton },
                { new Dictionary<Type, Type> { { typeof(IUnitOfWork), typeof(UnitOfWork) } }, Scope.Scope }
            };
        private static readonly DbContext dbContext = new BaseDbContext();

        public static void RegisterService<TInterface, TImplementation>(Scope scope)
        {
            registeredServices.Add(new Dictionary<Type, Type> { { typeof(TInterface), typeof(TImplementation) } }, scope);
        }

        public static Dictionary<Dictionary<Type, Type>, Scope> GetRegisteredServices()
        {
            return registeredServices;
        }

        public static ServiceLocator GetSingletonLocator()
        {
            return singleTonLocator;
        }

        public static DbContext Db
        {
            get
            {
                if (dbContext == null)
                {
                    throw new InvalidOperationException("DbContext is not initialized.");
                }
                return dbContext;
            }
        }
    }
}