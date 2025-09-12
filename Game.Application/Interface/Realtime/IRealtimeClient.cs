using System;
using System.Threading.Tasks;

namespace Game.Application.Interface.Realtime
{
    public interface IRealtimeClient
    {
        Task ConnectAsync(int timeoutMs = 5000);
        Task DisconnectAsync();
        Task<T> SendEventAsync<T>(string eventName, object data = null);
        void OnEvent(string eventName, Action<object> callback);
    }
}