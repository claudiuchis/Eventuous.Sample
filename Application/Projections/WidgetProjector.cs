using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
using Eventuous.Projections.MongoDB;

using static Eventuous.Sample.Domain.Events;

namespace Eventuous.Sample.Application.Projections
{
    public class WidgetProjector : MongoProjection<WidgetDetails>
    {
        public WidgetProjector(
            IMongoDatabase database,
            string subscriptionGroup,
            ILoggerFactory loggerFactory
        ) : base(database, subscriptionGroup, loggerFactory) {}

        protected override ValueTask<Operation<WidgetDetails>> GetUpdate(object @event, long? position)
        {
            return @event switch
            {
                V1.WidgetCreated created
                    => UpdateOperationTask(
                        created.WidgetId, 
                        u => u.Set(d => d.WidgetId, created.WidgetId)
                            .Set(d => d.WidgetName, created.WidgetName)
                    ),
                _ => NoOp
            };   
        }

    }
}