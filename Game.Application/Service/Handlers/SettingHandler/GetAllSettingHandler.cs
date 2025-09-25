using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Application.Domain.Entity;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Interface;
using Game.Application.Service.Models.Res.Settings;

namespace Game.Application.Service.Handlers.SettingHandler
{
    public class GetAllSettingReq { }

    [ReqModel(typeof(GetAllSettingReq))]
    public class GetAllSettingHandler : IServiceHandler<GetAllSettingReq, IEnumerable<SettingsRes>>
    {
        private readonly ILogger<GetAllSettingHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSettingHandler(ILogger<GetAllSettingHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(string, IEnumerable<SettingsRes>)> Handle(GetAllSettingReq request)
        {
            var query = await _unitOfWork.GetRepository<Setting>().QueryAll();

            return (string.Empty, query.Select(s => new SettingsRes
            {
                Key = s.Key,
                Value = s.Value
            }).ToList());
        }
    }
}