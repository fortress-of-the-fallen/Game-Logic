using System;
using System.Linq;
using System.Reflection;
using Game.Application.Interface;
using Game.Application.Service.Base.Locator;
using Game.Application.Service.Interface;

namespace Game.Application.Base.Mediator
{
    public class Mediator
    {
        public static TResponse Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
            using (var scopeLocator = new ServiceLocator(scope: Scope.Scope))
            {
                // Set the context for the request
                var contextAccessor = scopeLocator.Get<IContextAccessor>();
                contextAccessor.SetContextAccessor(request);

                // Get the type of the request
                var requestType = request.GetType();
                var handlerType = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t =>
                        t.GetInterfaces()
                         .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IServiceHandler<,>)) &&
                        t.GetCustomAttributes(typeof(ReqModelAttribute), true)
                         .OfType<ReqModelAttribute>()
                         .Any(attr => attr.RequestType == requestType)
                    );

                if (handlerType == null)
                    throw new Exception($"Handler not found for {requestType.Name}");

                var handler = (IServiceHandler<TRequest, TResponse>)scopeLocator.Get(handlerType);
                return handler.Handle(request);
            }
        }
    }
}