using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace FiveOhFirstDataCore.Data.Components
{
    public class InputEnumSelect<TEnum> : InputBase<TEnum>
    {
        [Parameter]
        public TEnum[]? Whitelist { get; set; }

        [Parameter]
        public TEnum[]? Blacklist { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "select");

            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder(this,
                value => CurrentValueAsString = value,
                CurrentValueAsString,
                CultureInfo.CurrentCulture));

            var typeSet = GetValueType();
            var type = typeSet.Item1;
            bool isNullable = typeSet.Item2;
            if (isNullable)
            {
                builder.OpenElement(5, "option");

                builder.AddAttribute(6, "value", "");
                builder.AddContent(7, "N/A");

                builder.CloseElement();
            }

            foreach (TEnum value in Enum.GetValues(type))
            {
                if (Whitelist is not null)
                    if (!Whitelist.Contains(value)) continue;

                if (Blacklist is not null)
                    if (Blacklist.Contains(value)) continue;

                builder.OpenElement(5, "option");

                var name = Enum.GetName(type, value);
                builder.AddAttribute(6, "value", name);
                builder.AddContent(7, GetDisplayName(value, type, name));

                builder.CloseElement();
            }

            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TEnum result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            if (BindConverter.TryConvertTo(value, CultureInfo.CurrentCulture, out result))
            {
                validationErrorMessage = null;
                return true;
            }

            if (string.IsNullOrEmpty(value))
            {
                var nullable = Nullable.GetUnderlyingType(typeof(TEnum));
                if (nullable is not null)
                {
                    result = default;
                    validationErrorMessage = null;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
                    // Value is nullable in this instance.
                    return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
                }
            }

            result = default;
            validationErrorMessage = $"Value is not an enum of type {GetValueType().Item1.Name}";
            return false;
        }

        private static (Type, bool) GetValueType()
        {
            var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));
            if (nullableType != null)
                return (nullableType, true);

            return (typeof(TEnum), false);
        }

        private string GetDisplayName(TEnum obj, Type type, string? name)
        {
            if (obj is null || name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttributes<DescriptionAttribute>()
                .SingleOrDefault()
                ?.Description ?? "";
        }
    }
}
