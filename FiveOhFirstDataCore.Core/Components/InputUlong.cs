using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Components;

public class InputUlong : InputBase<ulong>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "type", "number");
        builder.AddAttribute(2, "min", 0);
        builder.AddMultipleAttributes(3, AdditionalAttributes);
        builder.AddAttribute(4, "class", $"{CssClass}");
        builder.AddAttribute(5, "value", CurrentValueAsString);
        builder.AddAttribute(6, "oninput", EventCallback.Factory.Create(this, (x) =>
        {
            var tmp = (string?)x.Value ?? CurrentValueAsString;
            if (TryParseValueFromString(tmp, out var res, out var err))
            {
                CurrentValue = res;
            }
            else
            {
                CurrentValue = 0;
            }

            CurrentValueAsString = tmp;
        }));
        builder.CloseElement();
    }

    protected override bool TryParseValueFromString(string? value, 
        [MaybeNullWhen(false)] out ulong result, 
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if(ulong.TryParse(value, out var num))
        {
            result = num;
            validationErrorMessage = null;
            return true;
        }
        else
        {
            result = 0;
            validationErrorMessage = "Invalid Ulong provided.";
            return false;
        }
    }
}
