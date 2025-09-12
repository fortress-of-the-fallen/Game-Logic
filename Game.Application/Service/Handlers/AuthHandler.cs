using System.Net;
using System.Threading.Tasks;
using Game.Application.Domain.Message;
using Game.Application.Domain.Constant;
using Game.Application.Infra.Http;
using System.Collections.Generic;

namespace Game.Application.Service.Handlers
{
    public class AuthHandler
    {
        private readonly RestfulService _rest;

        public AuthHandler(RestfulService rest)
        {
            _rest = rest;
        }

        public async Task<ExecutionRes<object>> RegisterAsync(RegisterReq req)
        {
            var (result, status) = await _rest.Post<ExecutionRes<object>>(RouteConstant.Auth.Register, req);
            return HandleResponse(result, status);
        }

        public async Task<ExecutionRes<object>> LoginAsync(LoginReq req)
        {
            var (result, status) = await _rest.Post<ExecutionRes<object>>(RouteConstant.Auth.Login, req);
            return HandleResponse(result, status);
        }

        public async Task<ExecutionRes<object>> GithubLoginAsync()
        {
            var (result, status) = await _rest.Post<ExecutionRes<object>>(RouteConstant.Auth.GithubLogin, null);
            return HandleResponse(result, status);
        }

        public async Task<ExecutionRes<object>> LogoutAsync(string sessionId)
        {
            var headers = new Dictionary<string, string> { { "session-id", sessionId } };
            var (result, status) = await _rest.Post<ExecutionRes<object>>(RouteConstant.Auth.Logout, null, headers);
            return HandleResponse(result, status);
        }

        private ExecutionRes<object> HandleResponse(ExecutionRes<object> result, HttpStatusCode status)
        {
            if (status == HttpStatusCode.OK && result != null)
                return result;

            return new ExecutionRes<object>
            {
                Success = false,
                Error = $"Request failed with status {status}"
            };
        }
    }
}
