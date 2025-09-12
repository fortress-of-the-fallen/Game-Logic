using System;
using System.Threading.Tasks;
using Game.Application.Domain.Constant;
using Game.Application.Interface.Realtime;

namespace Game.Application.Infra.Realtime
{
    public class RealtimeClient : IRealtimeClient
    {
        private SocketIOClient.SocketIO _socket;
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

            _socket = new SocketIOClient.SocketIO(ConfigConstant.Url + _route, new SocketIOClient.SocketIOOptions
            {
                Reconnection = true,
                ConnectionTimeout = TimeSpan.FromMilliseconds(timeoutMs)
            });

            var tcs = new TaskCompletionSource<bool>();

            _socket.OnConnected += (sender, e) =>
            {
                _isConnected = true;
                Console.WriteLine("Socket connected");
                tcs.TrySetResult(true);
            };

            _socket.OnDisconnected += (sender, e) =>
            {
                _isConnected = false;
                Disconnected?.Invoke();
                Console.WriteLine("Socket disconnected");
            };

            _socket.OnError += (sender, e) =>
            {
                _isConnected = false;
                tcs.TrySetException(new Exception($"Socket error: {e}"));
                Console.WriteLine("Socket error: " + e);
            };

            await _socket.ConnectAsync();

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
            if (completedTask != tcs.Task)
                throw new TimeoutException("Socket connect timed out");

            await tcs.Task;
        }

        public async Task DisconnectAsync()
        {
            if (_socket != null && _isConnected)
            {
                await _socket.DisconnectAsync();
                _isConnected = false;
                Console.WriteLine("Socket disconnected");
            }
        }


        public void OnEvent(string eventName, Action<object> callback)
        {
            _socket?.On(eventName, callback);
        }

        public async Task<T> SendEventAsync<T>(string eventName, object data = null)
        {
            if (_socket == null || !_isConnected)
                throw new InvalidOperationException("Socket not connected.");

            var tcs = new TaskCompletionSource<T>();

            if (data != null)
            {
                await _socket.EmitAsync(eventName, response =>
                {
                    try
                    {
                        T value = response.GetValue<T>();
                        tcs.TrySetResult(value);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }, data);
            }
            else
            {
                await _socket.EmitAsync(eventName, response =>
                {
                    try
                    {
                        T value = response.GetValue<T>();
                        tcs.TrySetResult(value);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });
            }

            return await tcs.Task;
        }
    }
}
