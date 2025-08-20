using System.Threading.Tasks;
using Game.Application.Command.Models.Req.Sample;
using Game.Application.Interface;
using Game.Application.Interface.DataAccess.UnitOfWork;
using Game.Application.Service.Interface;

namespace Game.Application.Service.Handlers.Sample
{
    [ReqModel(typeof(SampleReq))]
    public class SampleHandler : IServiceHandler<SampleReq, Task<string>>
    {
        private readonly ILogger<SampleHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SampleHandler(ILogger<SampleHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(SampleReq request)
        {
            var sampleRepo = _unitOfWork.GetRepository<Domain.Entity.Sample>();

            await sampleRepo.Add(new Domain.Entity.Sample
            {
                Name = "request.Name",
                Description = "request.Description"
            });

            await _unitOfWork.SaveChanges();

            _logger.LogInformation("Handling SampleReq");
            return "Sample response from SampleHandler";
        }
    }
}