using System;

namespace Game.Application.Interface.Realtime
{
    public interface IRealtimeClient
    {
        void Connect(string url);
        void Disconnect();
        void SendEvent(string eventName, object data, Action<object> ack = null);
        void OnEvent(string eventName, Action<object> callback);
    }
}