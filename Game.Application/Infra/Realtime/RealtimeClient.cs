using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Game.Application.Domain.Constant;
using Game.Application.Domain.Helpers;
using Game.Application.Interface.Realtime;
using SocketIO.Core;
using SocketIOClient;

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
                ConnectionTimeout = TimeSpan.FromMilliseconds(timeoutMs),
                EIO = EngineIO.V4,
                ReconnectionDelay = 1000
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

        public async Task<T> SendEventAsync<T>(string eventName, object data = null, int timeoutMs = 10000)
        {
            if (_socket == null || !_socket.Connected)
                throw new InvalidOperationException("Socket not connected.");

            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            var cts = new CancellationTokenSource(timeoutMs);

            try
            {
                // Timeout
                cts.Token.Register(() =>
                    tcs.TrySetException(new TimeoutException(
                        $"No ack received for event '{eventName}' within {timeoutMs}ms")));

                // Ack callback
                Func<SocketIOResponse, Task> ack = response =>
                {
                    try
                    {
                        var jsonElement = response.GetValue<JsonElement>();
                        string connectionId = jsonElement.GetProperty("connectionId").GetString();
                        T value = response.GetValue<T>();
                        tcs.TrySetResult(value);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                    return Task.CompletedTask;
                };


                object[] payload = data != null ? new object[] { data } : Array.Empty<object>();
                await _socket.EmitAsync(eventName, ack, payload);
                return await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}
