using System.Threading.Tasks;

using Eventuous.Sample.Domain;

namespace Eventuous.Sample.Infrastructure
{
    public class ExternalService : IExternalService
    {
        public async Task ExecuteAsync()
        {
            await Task.Delay(1);
        }
    }
}