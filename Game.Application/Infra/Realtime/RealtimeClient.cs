using System;
using System.Threading.Tasks;
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

        public async Task ConnectAsync(int timeoutMs = 5000)
        {
            if (_socket != null && _isConnected)
                return;

            var tcs = new TaskCompletionSource<bool>();
            var url = ConfigConstant.Url + _route;

            _socket = IO.Socket(url);

            _socket.On(Socket.EVENT_CONNECT, () =>
            {
                _isConnected = true;
                tcs.TrySetResult(true);
                Console.WriteLine("Socket connected");
            });

            _socket.On(Socket.EVENT_ERROR, (error) =>
            {
                _isConnected = false;
                tcs.TrySetException(new Exception($"Socket error: {error}"));
                Console.WriteLine("Socket error: " + error);
            });

            _socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                _isConnected = false;
                Disconnected?.Invoke();
                Console.WriteLine("Socket disconnected");
            });

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
            if (completedTask != tcs.Task)
                throw new TimeoutException("Socket connect timed out");

            await tcs.Task; 
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
