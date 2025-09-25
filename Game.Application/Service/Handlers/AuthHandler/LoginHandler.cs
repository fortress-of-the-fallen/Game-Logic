using System.Threading.Tasks;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Handlers.SettingHandler;
using Game.Application.Service.Interface;
using Game.Application.Domain.Entity;
using Game.Application.Interface.Realtime;
using Newtonsoft.Json.Linq;
using Game.Application.Service.Models.Res.Base;
using Game.Application.Domain.Constant;
using System.Collections.Generic;

namespace Game.Application.Service.Handlers.AuthHandler
{
    public class LoginReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ConnectionId { get; set; }
    }

    [ReqModel(typeof(LoginReq))]
    public class LoginHandler : IServiceHandler<LoginReq, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRestfulService _restfulService;
        private string _connectionId;

        public LoginHandler(
            IUnitOfWork unitOfWork,
            IRestfulService restfulService)
        {
            _unitOfWork = unitOfWork;
            _restfulService = restfulService;
        }

        public async Task<(string, string)> Handle(LoginReq request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var loginData = new
            {
                username = request.Username,
                password = request.Password,
                rememberMe = request.RememberMe,
                connectionId = request.ConnectionId
            };

            var (loginRes, code) = await _restfulService.Post<ResultRes<string>>(ConfigConstant.Url + RouteConstant.Auth.Login, loginData);

            if (loginRes.IsSuccess == true)
            {
                if (await userRepo.Any(U => U.SessionId == loginRes.Result)) return (string.Empty, string.Empty);
                await userRepo.Add(new User { SessionId = loginRes.Result });
                await _unitOfWork.SaveChanges();
            }

            return (string.Empty, string.Empty);
        }
    }
}