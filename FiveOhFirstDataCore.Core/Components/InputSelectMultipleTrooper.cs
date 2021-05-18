using FiveOhFirstDataCore.Core.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components
{
    public class InputSelectMultipleTrooper : InputBase<List<Trooper>>
    {
        [Parameter]
        public List<Trooper> Troopers { get; set; } = new();

        private string DisplayValue { get; set; } = ""; 
        private List<string> InvalidSearches { get; set; } = new();
        private bool Valid { get; set; } = false;
        private List<Trooper> Suggestions { get; set; } = new();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (CurrentValue is not null && CurrentValue.Count != 0)
            {
                Valid = true;
            }

            builder.OpenElement(0, "div");

            builder.OpenElement(1, "input");
            builder.AddMultipleAttributes(2, AdditionalAttributes);
            builder.AddAttribute(3, "class", $"{CssClass} {(Valid ? "is-valid" : "is-invalid")}");
            builder.AddAttribute(4, "autocomplete", "off");
            builder.AddAttribute(5, "type", "text");
            builder.AddAttribute(6, "value", DisplayValue);
            builder.AddAttribute(7, "oninput", EventCallback.Factory.Create(this, (y) =>
            {
                DisplayValue = ((string?)y.Value) ?? "";

                var item = Troopers.FirstOrDefault(x => x.Id.ToString().Equals(DisplayValue, StringComparison.OrdinalIgnoreCase)
                    || x.NickName.Equals(DisplayValue, StringComparison.OrdinalIgnoreCase));

                Suggestions.Clear();

                if (item is not null)
                {
                    if (CurrentValue is null) CurrentValue = new();
                    if (CurrentValue.FirstOrDefault(x => x.Id == item.Id) is null)
                        CurrentValue.Add(item);
                    DisplayValue = "";
                }
                else if (!string.IsNullOrWhiteSpace(DisplayValue))
                {
                    Suggestions.AddRange(Troopers.Where(x => (x.Id.ToString().StartsWith(DisplayValue) || x.NickName.StartsWith(DisplayValue))
                        && !(CurrentValue?.Contains(x) ?? false)));
                }
                else
                {
                    Valid = false;
                }

                if (DisplayValue?.EndsWith(' ') ?? false)
                {
                    InvalidSearches.Add(DisplayValue);
                    DisplayValue = "";
                }
            }));
            builder.CloseElement();

            if (Suggestions.Count > 0)
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
                    builder.AddAttribute(15, "type", "button");
                    builder.AddAttribute(16, "onclick", EventCallback.Factory.Create(this, (x) =>
                    {
                        if (CurrentValue is null) CurrentValue = new();
                        if (CurrentValue.FirstOrDefault(x => x.Id == value.Id) is null)
                            CurrentValue.Add(value);
                        Valid = true;
                        DisplayValue = "";
                        Suggestions.Clear();
                    }));
                    builder.AddContent(17, $"{suggest.NickName} - {suggest.Id}");

                    builder.CloseElement();
                }

                builder.CloseElement();
                builder.CloseElement();
            }

            builder.OpenElement(18, "div");

            if (CurrentValue is not null)
            {
                builder.AddAttribute(19, "class", "p-1");

                for (int i = 0; i < CurrentValue.Count; i++)
                {
                    builder.OpenElement(20, "span");

                    builder.AddAttribute(21, "class", "btn btn-outline-success m-1");
                    builder.AddContent(22, CurrentValue[i].UserName);

                    builder.OpenElement(23, "button");
                    builder.AddAttribute(24, "type", "button");
                    builder.AddAttribute(25, "class", "oi oi-circle-x btn");

                    builder.AddAttribute(26, "onclick", GetRemoveFromCurrentEvent(i));

                    builder.CloseElement();
                    builder.CloseElement();
                }

                for (int i = 0; i < InvalidSearches.Count; i++)
                {
                    builder.OpenElement(27, "span");

                    builder.AddAttribute(28, "class", "btn btn-outline-danger m-1");
                    builder.AddContent(29, InvalidSearches[i]);

                    builder.OpenElement(30, "button");
                    builder.AddAttribute(31, "type", "button");
                    builder.AddAttribute(32, "class", "oi oi-circle-x btn");
                    builder.AddAttribute(33, "onclick", GetRemoveFromBadEvent(i));

                    builder.CloseElement();
                    builder.CloseElement();
                }
            }

            builder.CloseElement();
            builder.CloseElement();
        }

        protected EventCallback GetRemoveFromCurrentEvent(int toRemove)
        {
            return EventCallback.Factory.Create(this, (x) =>
            {
                if (CurrentValue is not null)
                {
                    CurrentValue.RemoveAt(toRemove);
                    StateHasChanged();
                }
            });
        }

        protected EventCallback GetRemoveFromBadEvent(int toRemove)
        {
            return EventCallback.Factory.Create(this, (x) =>
            {
                if (InvalidSearches is not null)
                {
                    InvalidSearches.RemoveAt(toRemove);
                    StateHasChanged();
                }
            });
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out List<Trooper> result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            throw new NotImplementedException();
        }

        protected override string? FormatValueAsString(List<Trooper>? value)
        {
            return base.FormatValueAsString(value);
        }
    }
}
