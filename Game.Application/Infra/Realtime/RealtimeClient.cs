using System;
using Game.Application.Domain.Constant;
using Game.Application.Interface.Realtime;
using Quobject.SocketIoClientDotNet.Client;

namespace Game.Application.Infra.Realtime
{
    public class RealtimeClient : IRealtimeClient
    {
        private Socket _socket;
        private bool _isConnected;
        private string _route;
        public event Action Disconnected;

        public RealtimeClient(string route)
        {
            _isConnected = false;
            _route = route;
        }

        public void Connect()
        {
            var url = ConfigConstant.Url + _route;

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
                Console.WriteLine("Socket disconnected");
            });

            _socket.On(Socket.EVENT_ERROR, (error) =>
            {
                _isConnected = false;
                Disconnected?.Invoke();
                Console.WriteLine("Socket error");
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

        public void SendEvent(string eventName, object data = null, Action<object> ack = null)
        {
            if (ack != null)
            {
                if (data != null)
                    _socket.Emit(eventName, ack, data);
                else
                    _socket.Emit(eventName, ack);
            }
            else
            {
                _socket.Emit(eventName, data);
            }
        }
    }
}
