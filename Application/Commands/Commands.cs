namespace Eventuous.Sample.Application
{
    public static class Commands
    {
        public record CreateWidget(
            string WidgetId
        );

        public record ReactWidget(
            string WidgetId
        );
    }
}