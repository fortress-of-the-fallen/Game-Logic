namespace Game.Application.Interface
{
    public interface IContextAccessor
    {
        void SetContextAccessor<TContext>(TContext context) where TContext : class;

        TContext GetContextAccessor<TContext>() where TContext : class;
    }
}