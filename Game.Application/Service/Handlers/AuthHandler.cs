using Game.Application.Domain.Message;
using Game.Application.Infra.Http;
using System.Threading.Tasks;
using System.Net;

namespace Game.Application.Service.Handlers
{
    public class AuthHandler
    {
        private readonly RestfulService _rest;

        public AuthHandler(RestfulService rest)
        {
            _rest = rest;
        }

        public async Task<LoginRes> LoginAsync(LoginReq req)
        {
            var (result, status) = await _rest.Post<LoginRes>("cho nay url nao", req);

            if (status == HttpStatusCode.OK && result != null)
            {
                return result;
            }

            return new LoginRes
            {
                Success = false,
                Error = $"Login failed with status {status}"
            };
        }
    }
}
