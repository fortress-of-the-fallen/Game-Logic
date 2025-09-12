using System;
using System.Threading.Tasks;

namespace Game.Application.Interface.Realtime
{
    public interface IRealtimeClient
    {
        Task ConnectAsync(int timeoutMs = 10000);
        Task DisconnectAsync();
        Task<string> SendEventAsync(string eventName, object data = null, int timeoutMs = 10000);
        void OnEvent(string eventName, Action<object> callback);
    }
}