using System.Threading.Tasks;

namespace Eventuous.Sample.Domain
{
    public interface IExternalService
    {
        Task ExecuteAsync();
    }
}