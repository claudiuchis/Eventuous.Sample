namespace Eventuous.Sample.Application
{
    public static class Commands
    {
        public record CreateWidget(
            string WidgetId,
            string WidgetName
        );

        public record ReactWidget(
            string WidgetId
        );
    }
}