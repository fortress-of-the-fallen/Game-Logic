namespace Game.Logic.Command.Base.Mediator
{
    public interface IRequestHandler<TRequest, TResponse>
    {
        TResponse Handle(TRequest request);
    }
}