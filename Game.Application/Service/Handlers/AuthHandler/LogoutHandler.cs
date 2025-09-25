using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Application.Domain.Constant;
using Game.Application.Domain.Entity;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Interface;
using Serilog;

namespace Game.Application.Service.Handlers.AuthHandler
{
    public class LogoutReq
    {
        public string SessionId { get; set; }
    }

    [ReqModel(typeof(LogoutReq))]
    public class LogoutHandler : IServiceHandler<LogoutReq, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRestfulService _restfulService;

        public LogoutHandler(
            IUnitOfWork unitOfWork,
            IRestfulService restfulService)
        {
            _unitOfWork = unitOfWork;
            _restfulService = restfulService;
        }

        public async Task<(string, string)> Handle(LogoutReq request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var logoutHeader = new Dictionary<string, string>
            {
                { "session-id", request.SessionId}
            };

            var (LogoutRes, code) = await _restfulService.Delete(ConfigConstant.Url + RouteConstant.Auth.Logout, logoutHeader);

            if (LogoutRes)
            {
                var user = await userRepo.Single(U => U.SessionId == request.SessionId);
                if (user != null)
                {
                    await userRepo.Delete(user);
                    await _unitOfWork.SaveChanges();    
                }
                
            }

            return (string.Empty, string.Empty);
        }
    }
}