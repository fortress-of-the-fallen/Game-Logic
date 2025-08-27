namespace Game.Application.Interface.Realtime
{
    public interface IRealtimeManager
    {
        IRealtimeClient GetClient(string route);

        void RemoveClient(string route);
    }
}