using System;
using Game.Application.Interface.Realtime;
using Quobject.SocketIoClientDotNet.Client;

namespace Game.Application.Infra.Realtime
{
    public class RealtimeClient : IRealtimeClient
    {
        private Socket _socket;
        private bool _isConnected;

        public event Action Disconnected;

        public void Connect(string url)
        {
            if (_socket != null && _isConnected)
                return;

            _socket = IO.Socket(url);

            _socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Socket connected");
                _isConnected = true;
            });

            _socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                _isConnected = false;
                Disconnected?.Invoke();
            });

            _socket.On(Socket.EVENT_ERROR, (error) =>
            {
                _isConnected = false;
                Disconnected?.Invoke();
            });
        }

        public void Disconnect()
        {
            _socket?.Disconnect();
        }

        public void OnEvent(string eventName, Action<object> callback)
        {
            _socket?.On(eventName, callback);
        }

        public void SendEvent(string eventName, object data, Action<object> ack = null)
        {
            _socket?.Emit(eventName, data, ack);
        }
    }
}
