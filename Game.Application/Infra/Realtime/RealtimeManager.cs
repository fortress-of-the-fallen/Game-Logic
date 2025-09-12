using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Application.Interface.Realtime;

namespace Game.Application.Infra.Realtime
{
    public class RealtimeManager : IRealtimeManager
    {
        private readonly Dictionary<string, IRealtimeClient> _clients = new Dictionary<string, IRealtimeClient>();

        public IRealtimeClient GetClient(string route)
        {
            if (_clients.TryGetValue(route, out var client))
            {
                return client;
            }

            client = new RealtimeClient(route);
            _clients[route] = client;
            return client;
        }

        public async Task RemoveClient(string route)
        {
            if (_clients.TryGetValue(route, out var client))
            {
                await client.DisconnectAsync();
                _clients.Remove(route);
            }
        }
    }
}