using System.Threading.Tasks;

namespace Game.Application.Service.Interface
{
    public interface IServiceHandler<TRequest, TResponse>
    {
        Task<(string, TResponse)> Handle(TRequest request);
    }
}