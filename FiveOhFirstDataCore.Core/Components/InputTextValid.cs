using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

using System.Diagnostics.CodeAnalysis;

namespace FiveOhFirstDataCore.Data.Components
{
    public class InputTextValid : InputBase<string>
    {
        [Parameter]
        public bool Valid { get; set; } = false;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", "text");
            builder.AddMultipleAttributes(2, AdditionalAttributes);
            builder.AddAttribute(3, "class", $"{CssClass} {(Valid ? "is-valid" : "is-invalid")}");
            builder.AddAttribute(4, "value", CurrentValueAsString);
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            result = value ?? "";
            validationErrorMessage = null;
            return true;
        }
    }
}
