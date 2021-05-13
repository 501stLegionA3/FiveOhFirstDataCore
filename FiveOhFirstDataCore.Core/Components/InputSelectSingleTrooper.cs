using FiveOhFirstDataCore.Core.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components
{
    public class InputSelectSingleTrooper : InputBase<Trooper>
    {
        [Parameter]
        public List<Trooper> Troopers { get; set; } = new();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out Trooper result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
