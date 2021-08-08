using System.Threading.Tasks;
using MongoDB.Driver;
using Eventuous.Sample.Application.Projections;

namespace Eventuous.Sample.Application.Queries
{
    public class WidgetQueryService
    {
        IMongoCollection<WidgetDetails> _database;
        public WidgetQueryService(
            IMongoDatabase database
        )
        {
            _database = database.GetCollection<WidgetDetails>(typeof(WidgetDetails).Name);
        }

        public async Task<WidgetDetails> GetWidgetById(string widgetId)
        {
            var query = await _database.FindAsync(d => d.Id == widgetId);
            return await query.SingleOrDefaultAsync();
        }
    }
}