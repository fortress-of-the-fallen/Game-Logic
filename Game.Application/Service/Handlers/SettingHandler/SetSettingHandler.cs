using System.Linq;
using System.Threading.Tasks;
using Game.Application.Domain.Constant;
using Game.Application.Domain.Entity;
using Game.Application.Domain.Message;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Interface;

namespace Game.Application.Service.Handlers.SettingHandler
{
    public sealed class SetSettingReq
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ReqModel(typeof(SetSettingReq))]
    public class SetSettingHandler : IServiceHandler<SetSettingReq, string>
    {
        private readonly ILogger<SetSettingHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SetSettingHandler(ILogger<SetSettingHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(string, string)> Handle(SetSettingReq request)
        {
            var settingRepo = _unitOfWork.GetRepository<Setting>();
            var setting = await settingRepo.Single(x => x.Key == request.Key);

            if (request.Key == SettingConstant.Language)
            {
                if (!LanguageConstant.Languages.Contains(request.Value))
                {
                    _logger.LogWarning($"Invalid language setting: {request.Value}");
                    return (SettingHandlerMessage.SetSettingHandler.InvalidLanguage, string.Empty);
                }
            }

            if (request.Value != setting.Value)
            {
                setting.Value = request.Value;
                await settingRepo.Update(setting);
                await _unitOfWork.SaveChanges();
            }

            return (string.Empty, string.Empty);
        }
    }
}