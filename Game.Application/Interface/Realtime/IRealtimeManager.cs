using System.Threading.Tasks;

namespace Game.Application.Interface.Realtime
{
    public interface IRealtimeManager
    {
        IRealtimeClient GetClient(string route);

        Task RemoveClient(string route);
    }
}