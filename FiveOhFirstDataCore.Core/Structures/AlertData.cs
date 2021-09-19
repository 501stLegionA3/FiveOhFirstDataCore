using FiveOhFirstDataCore.Data.Structures.Notice;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Data.Structures
{
    public class AlertData
    {
        public MarkupString Display => new(Content);
        public string Content { get; set; } = "";
        public AlertLevel Level { get; set; }
    }
}
