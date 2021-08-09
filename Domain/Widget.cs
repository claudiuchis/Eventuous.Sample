using System;
using System.Threading.Tasks;
using Eventuous;
using static Eventuous.Sample.Domain.Events;

namespace Eventuous.Sample.Domain
{
    public record WidgetId(string Value) : AggregateId(Value);

    public class Widget : Aggregate<WidgetState, WidgetId>
    {
        public void Create(WidgetId id, WidgetName name)
        {
            EnsureDoesntExist();
            Apply(new V1.WidgetCreated(id, name));
        }

        /*
            This is an example of an aggregate method which calls an external service, 
            e.g. send an email after a user signs up.
        */
        public async Task React(IExternalService service)
        {
            await service.ExecuteAsync();
            Apply(new V1.WidgetReacted(GetId()));
        }
    }

    public record WidgetState : AggregateState<WidgetState, WidgetId> 
    {
        public WidgetName Name { get; init; }
        public bool Reacted { get; init; }

        public override WidgetState When(object @event)
            => @event switch {
                V1.WidgetCreated created => this with {
                    Id = new WidgetId(created.WidgetId),
                    Name = new WidgetName(created.WidgetName),
                    Reacted = false
                },
                V1.WidgetReacted reacted => this with {
                    Reacted = true
                },
                _ => this
            };
    }
}