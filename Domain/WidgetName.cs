using System;
namespace Eventuous.Sample.Domain
{
    public record WidgetName
    {
        public string Value { get; init; }
        public WidgetName(string widgetName)
        {
            if (String.IsNullOrEmpty(widgetName))
                throw new ArgumentNullException(nameof(widgetName));
            Value = widgetName;
        }
        public static implicit operator string(WidgetName widgetName) => widgetName.Value;
    }
}