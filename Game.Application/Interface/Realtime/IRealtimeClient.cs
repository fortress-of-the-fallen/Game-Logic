using System;
using System.Threading.Tasks;

namespace Game.Application.Interface.Realtime
{
    public interface IRealtimeClient
    {
        Task ConnectAsync(int timeoutMs = 5000);
        void Disconnect();
        void SendEvent(string eventName, object data, Action<object> ack = null);
        void OnEvent(string eventName, Action<object> callback);
    }
}