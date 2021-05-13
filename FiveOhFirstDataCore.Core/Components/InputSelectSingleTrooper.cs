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
                DisplayValue = ((string?)x.Value) ?? "";

                var item = Troopers.FirstOrDefault(x => x.Id.ToString() == DisplayValue || x.NickName == DisplayName);

                if(item is not null)
                {
                    Valid = true;
                    Suggestions.Clear();
                    CurrentValue = item;
                    DisplayValue = item.NickName;
                }
                else if(!string.IsNullOrWhiteSpace(DisplayValue))
                {
                    Valid = false;
                    Suggestions.Clear();
                    Suggestions.AddRange(Troopers.Where(x => x.Id.ToString().StartsWith(DisplayValue) || x.NickName.StartsWith(DisplayValue)));
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
                builder.AddAttribute(12, "class", "table table-hover position-absolute top-100 start-50 bg-secondary");

                foreach (var suggest in Suggestions)
                {
                    builder.OpenElement(13, "button");

                    var suggestNick = suggest.NickName;
                    var value = suggest;

                    builder.AddAttribute(14, "class", "btn btn-primary btn-block");
                    builder.AddAttribute(15, "onclick", EventCallback.Factory.Create(this, (x) =>
                    {
                        CurrentValue = value;
                        Valid = true;
                        DisplayValue = suggestNick;
                    }));
                    builder.AddContent(16, $"{suggest.NickName} - {suggest.Id}");

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
