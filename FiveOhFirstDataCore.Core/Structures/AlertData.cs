using FiveOhFirstDataCore.Core.Data.Notice;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class AlertData
    {
        public MarkupString Display => new(Content);
        public string Content { get; set; } = "";
        public AlertLevel Level { get; set; }
    }
}
