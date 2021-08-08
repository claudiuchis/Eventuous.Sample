using System;
using Eventuous;
using Eventuous.Sample.Domain;
using static Eventuous.Sample.Application.Commands;

namespace Eventuous.Sample.Application
{
    public class WidgetService : ApplicationService<Widget, WidgetState, WidgetId>
    {
        public WidgetService(
            IAggregateStore store,
            IExternalService externalService
        ) : base(store)
        {
            OnAny<CreateWidget>(
                cmd => new WidgetId(cmd.WidgetId),
                (widget, cmd)
                    => widget.Create(new WidgetId(cmd.WidgetId))
            );

            OnExistingAsync<ReactWidget>(
                cmd => new WidgetId(cmd.WidgetId),
                async (widget, cmd, cancellationToken)
                    => await widget.React(externalService)
            );
        }
    }
}