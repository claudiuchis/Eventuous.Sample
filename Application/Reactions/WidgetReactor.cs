using System.Threading;
using System.Threading.Tasks;
using Eventuous.Subscriptions;

using static Eventuous.Sample.Domain.Events;
using static Eventuous.Sample.Application.Commands;

namespace Eventuous.Sample.Application.Reactions
{
    public class WidgetReactor : IEventHandler
    {
        private WidgetCommandService _widgetService;
        public string SubscriptionId { get; }

        public WidgetReactor(
            string subscriptionGroup,
            WidgetCommandService widgetService
        )
        {
            SubscriptionId = subscriptionGroup;
            _widgetService = widgetService;
        }

        public async Task HandleEvent(
            object @event, 
            long? position,
            CancellationToken cancellationToken
        )
        {
            var result = @event switch
            {
                V1.WidgetCreated created => _widgetService.Handle(
                    new ReactWidget(created.WidgetId),
                    cancellationToken
                ),
                _ => Task.CompletedTask
            };

            await result;
        }

    }
}