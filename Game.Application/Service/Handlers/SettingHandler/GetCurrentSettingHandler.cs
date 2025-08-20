using System.Threading.Tasks;
using Game.Application.Domain.Entity;
using Game.Application.Domain.Message;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Interface;

namespace Game.Application.Service.Handlers.SettingHandler
{
    /// <summary>
    /// SettingHandlerMessage.GetCurrentSettingHandler.SettingNotFound: Setting not found.
    /// </summary>
    public class GetCurrentSettingReq
    {
        public string Key { get; set; }
    }

    [ReqModel(typeof(GetCurrentSettingReq))]
    public class GetCurrentSettingHandler : IServiceHandler<GetCurrentSettingReq, string>
    {
        private readonly ILogger<GetCurrentSettingHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentSettingHandler(ILogger<GetCurrentSettingHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(string, string)> Handle(GetCurrentSettingReq request)
        {
            var settingRepo = _unitOfWork.GetRepository<Setting>();
            var setting = await settingRepo.Single(x => x.Key == request.Key);

            if (setting == null)
            {
                _logger.LogWarning($"Setting not found: {request.Key}");
                return (SettingHandlerMessage.GetCurrentSettingHandler.SettingNotFound, string.Empty);
            }

            return (string.Empty, setting.Value);
        }
    }
}