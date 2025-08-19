using Game.Application.Command.Models.Req.Sample;
using Game.Application.Interface;
using Game.Application.Service.Interface;

namespace Game.Application.Service.Handlers.Sample
{
    [ReqModel(typeof(SampleReq))]
    public class SampleHandler : IServiceHandler<SampleReq, string>
    {
        private readonly ILogger<SampleHandler> _logger;

        public SampleHandler(ILogger<SampleHandler> logger)
        {
            _logger = logger;
        }

        public string Handle(SampleReq request)
        {
            _logger.LogInformation("Handling SampleReq");
            return "Sample response from SampleHandler";
        }
    }
}