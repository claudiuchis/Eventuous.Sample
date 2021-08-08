namespace Eventuous.Sample.Domain
{
    public static class Events
    {
        public record WidgetCreated(string WidgetId);
        public record WidgetReacted(string WidgetId);
    }
}