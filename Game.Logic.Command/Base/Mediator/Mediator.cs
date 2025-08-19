using Game.Logic.Command.Base.Locator;

namespace Game.Logic.Command.Base.Mediator
{
    public class Mediator
    {
        public static TResponse Send<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
        {
            using (var scopeLocator = new ServiceLocator(scope: Scope.Scope))
            {
                // Determine the concrete handler type for this request/response pair
                var handlerType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));

                // Resolve the handler instance from the ServiceLocator
                var handler = (IRequestHandler<TRequest, TResponse>)scopeLocator.Get(handlerType);

                // Invoke the handler and return the response
                return handler.Handle(request);
            }
        }
    }
}