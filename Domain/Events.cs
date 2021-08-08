namespace Eventuous.Sample.Domain
{
    public static class Events
    {
        public static class V1
        {
            public record WidgetCreated(
                string WidgetId,
                string WidgetName
            );
            public record WidgetReacted(
                string WidgetId
            );
        }
    }
}