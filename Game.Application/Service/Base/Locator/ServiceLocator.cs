using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Application.Service.Base.Locator
{
    public interface IServiceLocator
    {
        T Get<T>(params object[] userArgs) where T : class;

        object Get(Type type, params object[] userArgs);
    }

    public class ServiceLocator : IServiceLocator, IDisposable
    {
        public ServiceLocator(Scope scope = Scope.Transient)
        {
            _scopeLevel = scope.GetScopeLevel();
        }

        private int _scopeLevel;
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly Dictionary<Dictionary<Type, Type>, Scope> _registeredServices = Dependencies.GetRegisteredServices();
        private readonly ServiceLocator _singleTonLocator = Dependencies.GetSingletonLocator();

        public T Get<T>(params object[] userArgs) where T : class
        {
            return (T)Get(typeof(T), userArgs);
        }

        public object Get(Type type, params object[] userArgs)
        {
            if (_services.ContainsKey(type))
                return _services[type];

            Scope scope = Scope.Transient;
            Type implType = type;

            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    var genericDefinition = type.GetGenericTypeDefinition();
                    var reg = _registeredServices.FirstOrDefault(d => d.Key.ContainsKey(genericDefinition));
                    if (reg.Key != null)
                    {
                        implType = reg.Key[genericDefinition].MakeGenericType(type.GetGenericArguments());
                        scope = reg.Value;

                        if (scope != Scope.Transient && scope.GetScopeLevel() < _scopeLevel)
                            throw new Exception($"Cannot inject {implType.Name} with lower scope than locator level {_scopeLevel}");
                    }
                    else
                    {
                        throw new Exception($"No mapping found for interface {type.Name}");
                    }
                }
                else
                {
                    var reg = _registeredServices.FirstOrDefault(d => d.Key.ContainsKey(type));
                    if (reg.Key != null)
                    {
                        implType = reg.Key[type];
                        scope = reg.Value;

                        if (scope != Scope.Transient && scope.GetScopeLevel() < _scopeLevel)
                            throw new Exception($"Cannot inject {implType.Name} with lower scope than locator level {_scopeLevel}");
                    }
                    else
                    {
                        throw new Exception($"No mapping found for interface {type.Name}");
                    }
                }
            }

            switch (scope)
            {
                case Scope.Singleton:
                    return _singleTonLocator.Get(implType, userArgs);

                case Scope.Scope:
                    var scopedInstance = CreateInstance(implType, userArgs);
                    _services[implType] = scopedInstance;
                    return scopedInstance;

                case Scope.Transient:
                    return CreateInstance(implType, userArgs);

                default:
                    throw new Exception($"Unknown scope {scope}");
            }
        }

        private object CreateInstance(Type type, object[] userArgs)
        {
            ConstructorInfo ctor = null;
            object[] args;

            if (userArgs != null && userArgs.Length > 0)
            {
                ctor = type.GetConstructors()
                           .FirstOrDefault(c => c.GetParameters().Length == userArgs.Length);
                if (ctor == null)
                    throw new Exception($"No constructor matching {userArgs.Length} args for {type.Name}");
                args = userArgs;
            }
            else
            {
                ctor = type.GetConstructors()
                           .OrderByDescending(c => c.GetParameters().Length)
                           .FirstOrDefault();
                if (ctor == null)
                    throw new Exception($"No public constructor found for {type.Name}");

                var parameters = ctor.GetParameters();
                args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    args[i] = Get(paramType); // resolve recursively
                }
            }

            return ctor.Invoke(args);
        }

        public void Dispose()
        {
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                    disposable.Dispose();
            }
            _services.Clear();
        }
    }
}