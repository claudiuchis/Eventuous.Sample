using System.Threading;
using System.Threading.Tasks;
using Eventuous.Subscriptions;

using static Eventuous.Sample.Domain.Events;
using static Eventuous.Sample.Application.Commands;

namespace Eventuous.Sample.Application
{
    public class WidgetReactor : IEventHandler
    {
        private WidgetService _widgetService;
        public string SubscriptionId { get; }

        public WidgetReactor(
            string subscriptionGroup,
            WidgetService widgetService
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
                WidgetCreated created => _widgetService.Handle(
                    new ReactWidget(created.WidgetId),
                    cancellationToken
                ),
                _ => Task.CompletedTask
            };

            await result;
        }

    }
}