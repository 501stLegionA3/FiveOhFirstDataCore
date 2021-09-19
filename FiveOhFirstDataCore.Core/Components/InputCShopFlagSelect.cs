using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

using System.Globalization;

namespace FiveOhFirstDataCore.Data.Components
{
    public class InputCShopFlagSelect : InputBase<CShop>
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", CssClass);
            builder.AddAttribute(2, "role", "group");
            builder.AddMultipleAttributes(3, AdditionalAttributes);
            builder.AddAttribute(4, "aria-label", "Qualifications button toggle group");

            var disabled = false;
            if (AdditionalAttributes is not null
                && AdditionalAttributes.TryGetValue("disabled", out var value))
            {
                try
                {
                    disabled = Convert.ToBoolean(value);
                }
                catch
                {
                    disabled = false;
                }
            }

            var quals = (CShop[])Enum.GetValues(typeof(CShop));

            for (int x = 1; x < quals.Length; x += 3)
            {
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "row");

                string classes = (quals.Length - x) switch
                {
                    1 => "col-lg-12 p-1",
                    2 => "col-lg-6 col-md-12 p-1",
                    _ => "col-lg-4 col-md-12 p-1"
                };

                for (int i = x; i < x + 3 && i < quals.Length; i++)
                {
                    builder.OpenElement(7, "div");
                    builder.AddAttribute(8, "class", classes);

                    builder.OpenElement(9, "button");
                    builder.AddAttribute(10, "class", $"w-100 p-1 {((CurrentValue & quals[i]) == quals[i] ? "btn-success text-light" : "btn-outline-danger text-dark")} btn");
                    builder.AddAttribute(11, "onclick", GetEventCallback(quals[i]));
                    builder.AddAttribute(12, "disabled", disabled);
                    builder.AddContent(13, quals[i].AsFull());

                    builder.CloseElement();
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            builder.CloseElement();
        }

        private EventCallback GetEventCallback(CShop qual)
        {
            return EventCallback.Factory.Create(this, (x) =>
            {
                if ((CurrentValue & qual) == qual)
                    CurrentValue ^= qual;
                else
                    CurrentValue |= qual;
            });
        }

        protected override bool TryParseValueFromString(string value, out CShop result, out string validationErrorMessage)
        {
            // Let's Blazor convert the value for us 😊
            if (BindConverter.TryConvertTo(value, CultureInfo.CurrentCulture, out CShop parsedValue))
            {
                result = parsedValue;
                validationErrorMessage = null;
                return true;
            }

            // Map null/empty value to null if the bound object is nullable
            if (string.IsNullOrEmpty(value))
            {
                var nullableType = Nullable.GetUnderlyingType(typeof(CShop));
                if (nullableType != null)
                {
                    result = default;
                    validationErrorMessage = null;
                    return true;
                }
            }

            // The value is invalid => set the error message
            result = default;
            validationErrorMessage = $"The {FieldIdentifier.FieldName} field is not valid.";
            return false;
        }
    }
}
