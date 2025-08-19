namespace Game.Application.Service.Interface
{
    public interface IServiceHandler<TRequest, TResponse>
    {
        TResponse Handle(TRequest request);
    }
}