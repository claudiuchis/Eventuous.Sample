using Eventuous.Projections.MongoDB.Tools;

namespace Eventuous.Sample.Application.Projections
{
    public record WidgetDetails(
        string WidgetId, 
        string WidgetName
    ) : ProjectedDocument(WidgetId);
}