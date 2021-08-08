using System;
using Eventuous;
using static Eventuous.Sample.Domain.Events;

namespace Eventuous.Sample
{
    internal static class EventMapping
    {
        public static void MapEventTypes()
        {
            TypeMap.AddType<WidgetCreated>("WidgetCreated");
            TypeMap.AddType<WidgetReacted>("WidgetReacted");
        }
    }
}