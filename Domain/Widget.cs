using System;
using System.Threading.Tasks;
using Eventuous;
using static Eventuous.Sample.Domain.Events;

namespace Eventuous.Sample.Domain
{
    public record WidgetId(string Value) : AggregateId(Value);

    public class Widget : Aggregate<WidgetState, WidgetId>
    {
        public void Create(WidgetId id)
        {
            EnsureDoesntExist();
            Apply(new WidgetCreated(id));
        }

        public async Task React(IExternalService service)
        {
            await service.ExecuteAsync();
            Apply(new WidgetReacted(GetId()));
        }
    }

    public record WidgetState : AggregateState<WidgetState, WidgetId> 
    {
        public override WidgetState When(object @event)
            => @event switch {
                WidgetCreated created => this with {
                    Id = new WidgetId(created.WidgetId)
                },
                WidgetReacted reacted => this,
                _ => this
            };
    }
}