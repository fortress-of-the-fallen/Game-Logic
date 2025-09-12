using System.Threading.Tasks;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Handlers.SettingHandler;
using Game.Application.Service.Interface;
using Game.Application.Domain.Entity;
using Game.Application.Interface.Realtime;
using Newtonsoft.Json.Linq;

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
        private readonly IRestfulService _restfulService;
        private string _connectionId;

        public LoginHandler(
            ILogger<SetSettingHandler> logger,
            IUnitOfWork unitOfWork,
            IRealtimeManager realTimeManager,
            IRestfulService restfulService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _realTimeManager = realTimeManager;
            _restfulService = restfulService;
        }

        public async Task<(string, string)> Handle(LoginReq request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var loginClient = _realTimeManager.GetClient("/login");

            await loginClient.ConnectAsync();

            var tcs = new TaskCompletionSource<string>();
            loginClient.OnEvent("message", response =>
            {
                tcs.TrySetResult(response.ToString());
            });

            var resp = await loginClient.SendEventAsync("getConnectionId");
            JArray arr = JArray.Parse(resp);
            string connectionId = (string)arr[0]["connectionId"];

            await Task.WhenAny(tcs.Task, Task.Delay(100000));

            return (_connectionId, string.Empty);
        }
    }
}