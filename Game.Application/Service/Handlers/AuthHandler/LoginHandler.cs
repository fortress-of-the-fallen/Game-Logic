using System.Threading.Tasks;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Handlers.SettingHandler;
using Game.Application.Service.Interface;
using Game.Application.Infra.Realtime;
using Game.Application.Domain.Entity;
using Game.Application.Interface.Realtime;
using System;
using Game.Application.Domain.Constant;

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
        private readonly ILogger<SetSettingHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRealtimeManager _realTimeManager;
        private string _connectionId;

        public LoginHandler(ILogger<SetSettingHandler> logger, IUnitOfWork unitOfWork, IRealtimeManager realTimeManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _realTimeManager = realTimeManager;
        }

        public async Task<(string, string)> Handle(LoginReq request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var loginClient = _realTimeManager.GetClient("/login");
            loginClient.Connect();
            loginClient.OnEvent("getConnectionId", response =>
            {
                var resp = response as dynamic;
                _connectionId = resp.connectionId;
            });
            loginClient.SendEvent("getConnectionId", null);

            return (string.Empty, string.Empty);
        }
    }
}