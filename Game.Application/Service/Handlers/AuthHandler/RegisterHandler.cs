using System.Threading.Tasks;
using Game.Application.Domain.Constant;
using Game.Application.Domain.Entity;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Interface;
using Game.Application.Service.Models.Res.Base;
using Serilog.Core;

namespace Game.Application.Service.Handlers.AuthHandler
{
    public class RegisterReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    [ReqModel(typeof(RegisterReq))]
    public class RegisterHandler : IServiceHandler<RegisterReq, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRestfulService _restfulService;

        public RegisterHandler(
            IUnitOfWork unitOfWork,
            IRestfulService restfulService)
        {
            _unitOfWork = unitOfWork;
            _restfulService = restfulService;
        }

        public async Task<(string, string)> Handle(RegisterReq request)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            var registerBody = new
            {
                username = request.Username,
                password = request.Password,
                confirmPassword = request.ConfirmPassword
            };

            var (registerRes, code) = await _restfulService.Post<ResultRes<string>>(ConfigConstant.Url + RouteConstant.Auth.Register, registerBody);

            if (registerRes.IsSuccess == true)
            {
                if (await userRepo.Any(U => U.SessionId == registerRes.Result)) return (string.Empty, string.Empty);
                await userRepo.Add(new User { SessionId = registerRes.Result });
                await _unitOfWork.SaveChanges();
            }

            return (string.Empty, string.Empty);
            
        }
    }
}