using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Misc
{
    public partial class ErrorSuccessNotice
    {
        [Parameter]
        public List<string> Errors { get; set; } = new();
        [Parameter]
        public Action ClearErrors { get; set; }

        [Parameter]
        public string? SuccessMessage { get; set; }
        [Parameter]
        public Action ClearSuccess { get; set; }
    }
}
