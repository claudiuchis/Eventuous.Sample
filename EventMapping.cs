using System;
using Eventuous;
using static Eventuous.Sample.Domain.Events;

namespace Eventuous.Sample
{
    internal static class EventMapping
    {
        public static void MapEventTypes()
        {
            TypeMap.AddType<V1.WidgetCreated>("WidgetCreated");
            TypeMap.AddType<V1.WidgetReacted>("WidgetReacted");
        }
    }
}