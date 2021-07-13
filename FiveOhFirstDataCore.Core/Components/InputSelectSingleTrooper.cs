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

        private string DisplayValue { get; set; } = "";
        private bool Valid { get; set; } = false;
        private List<Trooper> Suggestions { get; set; } = new();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if(CurrentValue is not null && CurrentValue.Id != 0)
            {
                DisplayValue = CurrentValue.NickName;
                Valid = true;
            }

            builder.OpenElement(0, "div");

            builder.OpenElement(1, "input");
            builder.AddMultipleAttributes(2, AdditionalAttributes);
            builder.AddAttribute(3, "class", $"{CssClass} {(Valid ? "is-valid" : "is-invalid")}");
            builder.AddAttribute(4, "autocomplete", "off");
            builder.AddAttribute(5, "type", "text");
            builder.AddAttribute(6, "value", DisplayValue);
            builder.AddAttribute(7, "oninput", EventCallback.Factory.Create(this, (x) =>
            {
                var display = (string?)x.Value ?? "";

                var item = Troopers.FirstOrDefault(x => 
                {
                    if (x is null) return false;

                    return x.Id.ToString().Equals(display.Trim(), StringComparison.OrdinalIgnoreCase)
                        || x.NickName.Equals(display.Trim(), StringComparison.OrdinalIgnoreCase);
                });

            if (item is not null)
            {
                Valid = true;
                Suggestions.Clear();
            }
            else if (!string.IsNullOrWhiteSpace(display))
            {
                Valid = false;
                Suggestions.Clear();
                    Suggestions.AddRange(Troopers.Where(x =>
                    {
                        if (x is null) return false;
                        return x.Id.ToString().StartsWith(display, StringComparison.OrdinalIgnoreCase)
                            || x.NickName.StartsWith(display, StringComparison.OrdinalIgnoreCase);
                    }));
                }
                else
                {
                    Valid = false;
                    Suggestions.Clear();
                }
            }));
            builder.CloseElement();

            if(Suggestions.Count > 0)
            {
                builder.OpenElement(8, "div");
                builder.AddAttribute(9, "class", "position-relative bg-secondary");
                builder.AddAttribute(10, "style", "z-index: 2000");
                builder.OpenElement(11, "div");
                builder.AddAttribute(12, "class", "table table-hover position-absolute end-0 start-0 bg-secondary");

                foreach (var suggest in Suggestions)
                {
                    builder.OpenElement(13, "div");
                    builder.AddAttribute(14, "class", "d-grid gap-2");
                    builder.OpenElement(15, "button");

                    var suggestNick = suggest.NickName;
                    var value = suggest;

                    builder.AddAttribute(16, "class", "btn btn-primary");
                    builder.AddAttribute(17, "onclick", EventCallback.Factory.Create(this, (x) =>
                    {
                        CurrentValue = value;
                        Valid = true;
                        DisplayValue = suggestNick;
                        Suggestions.Clear();
                    }));
                    builder.AddContent(18, $"{suggest.NickName} - {suggest.Id}");

                    builder.CloseElement();
                    builder.CloseElement();
                }

                builder.CloseElement();
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out Trooper result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            throw new NotImplementedException();
        }

        protected override string? FormatValueAsString(Trooper? value)
        {
            return value?.NickName ?? "";
        }
    }
}
