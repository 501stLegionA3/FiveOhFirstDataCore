using FiveOhFirstDataCore.Core.Data.Notice;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class AlertData
    {
        public MarkupString Display => new(Content);
        public string Content { get; set; } = "";
        public AlertLevel Level { get; set; }
    }
}
