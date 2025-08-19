using System;
using System.Collections.Generic;
using Game.Logic.Command.Base.Locator;

namespace Game.Logic.Command
{
    public static class Dependencies
    {
        private static readonly ServiceLocator singleTonLocator = new ServiceLocator(scope: Scope.Singleton);
        private static readonly Dictionary<Dictionary<Type, Type>, Scope> registeredServices = new Dictionary<Dictionary<Type, Type>, Scope>();

        public static void AddDependency<TInterface, TImplementation>(Scope scope)
            where TImplementation : TInterface
        {
            var key = new Dictionary<Type, Type>
            {
                { typeof(TInterface), typeof(TImplementation) }
            };
            registeredServices[key] = scope;
        }

        public static Dictionary<Dictionary<Type, Type>, Scope> GetRegisteredServices()
        {
            return registeredServices;
        }

        public static ServiceLocator GetSingletonLocator()
        {
            return singleTonLocator;
        }
    }
}