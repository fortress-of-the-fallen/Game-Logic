using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Application.Domain.Message;
using Game.Application.Interface;
using Game.Application.Service.Base.Locator;
using Game.Application.Service.Interface;
using Game.Application.Service.Models.Res.Base;

namespace Game.Application.Base.Mediator
{
    public class Mediator
    {
        public async static Task<ResultRes<T>> Send<TRequest, T>(TRequest request)
            where TRequest : class
            where T : class
        {
            using (var scopeLocator = new ServiceLocator(scope: Scope.Scope))
            {
                var contextAccessor = scopeLocator.Get<IContextAccessor>();
                contextAccessor.SetContextAccessor(request);

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
                    return ResultRes<T>.Fail($"Handler not found for {requestType.Name}");

                var handler = (IServiceHandler<TRequest,  T>)scopeLocator.Get(handlerType);

                try
                {
                    var (errorCode, data) = await handler.Handle(request);
                    return ResultRes<T>.Get(errorCode, data);
                }
                catch (Exception ex)
                {
                    var logger = scopeLocator.Get<ILogger<Mediator>>();
                    logger.LogError($"Error occurred while handling {requestType.Name}, {ex.Message}");
                    return ResultRes<T>.Fail(BaseMessage.Exception);
                }
            }
        }
    }
}
