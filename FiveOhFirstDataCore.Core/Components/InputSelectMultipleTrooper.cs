using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

using System.Diagnostics.CodeAnalysis;

namespace FiveOhFirstDataCore.Data.Components
{
    public class InputSelectMultipleTrooper : InputBase<List<Trooper>>
    {
        [Parameter]
        public List<Trooper> Troopers { get; set; } = new();

        private bool ResetDisplay { get; set; } = false;
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
            if (ResetDisplay)
            {
                builder.AddAttribute(6, "value", "");
            }
            builder.AddAttribute(7, "oninput", EventCallback.Factory.Create(this, (y) =>
            {
                string display = ((string?)y.Value) ?? "";

                var item = Troopers.FirstOrDefault(x => x.BirthNumber.ToString().Equals(display.Trim(), StringComparison.OrdinalIgnoreCase)
                    || x.NickName.Equals(display.Trim(), StringComparison.OrdinalIgnoreCase));

                Suggestions.Clear();

                if (CurrentValue is null) CurrentValue = new();

                if (item is not null || !string.IsNullOrWhiteSpace(display))
                {
                    Suggestions.AddRange(Troopers.Where(x => (x.BirthNumber.ToString().StartsWith(display, StringComparison.OrdinalIgnoreCase)
                        || x.NickName.StartsWith(display, StringComparison.OrdinalIgnoreCase))
                        && !(CurrentValue.Contains(x))));
                }
                else
                {
                    Valid = false;
                }

                if (display?.EndsWith(' ') ?? false)
                {
                    if (item is not null && !CurrentValue.Contains(item))
                    {
                        if (CurrentValue is null) CurrentValue = new();
                        CurrentValue.Add(item);
                    }
                    else
                    {
                        InvalidSearches.Add(display);
                    }

                    ResetDisplay = !ResetDisplay;
                }
            }));
            builder.CloseElement();

            if (Suggestions.Count > 0)
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
                    builder.AddAttribute(17, "type", "submit");
                    builder.AddAttribute(18, "onclick", EventCallback.Factory.Create(this, (x) =>
                    {
                        if (CurrentValue is null) CurrentValue = new();
                        if (CurrentValue.FirstOrDefault(x => x.BirthNumber == value.BirthNumber) is null)
                            CurrentValue.Add(value);
                        Valid = true;
                        ResetDisplay = !ResetDisplay;
                        Suggestions.Clear();
                    }));
                    builder.AddContent(19, $"{suggest.NickName} - {suggest.BirthNumber}");

                    builder.CloseElement();
                    builder.CloseElement();
                }

                builder.CloseElement();
                builder.CloseElement();
            }

            builder.OpenElement(20, "div");

            if (CurrentValue is not null)
            {
                builder.AddAttribute(21, "class", "p-1");

                for (int i = 0; i < CurrentValue.Count; i++)
                {
                    builder.OpenElement(22, "span");

                    builder.AddAttribute(23, "class", "btn btn-outline-success m-1");
                    builder.AddContent(24, CurrentValue[i].NickName);

                    builder.OpenElement(25, "button");
                    builder.AddAttribute(26, "type", "button");
                    builder.AddAttribute(27, "class", "oi oi-circle-x btn");

                    builder.AddAttribute(28, "onclick", GetRemoveFromCurrentEvent(i));

                    builder.CloseElement();
                    builder.CloseElement();
                }

                for (int i = 0; i < InvalidSearches.Count; i++)
                {
                    builder.OpenElement(29, "span");

                    builder.AddAttribute(30, "class", "btn btn-outline-danger m-1");
                    builder.AddContent(31, InvalidSearches[i]);

                    builder.OpenElement(32, "button");
                    builder.AddAttribute(33, "type", "button");
                    builder.AddAttribute(34, "class", "oi oi-circle-x btn");
                    builder.AddAttribute(35, "onclick", GetRemoveFromBadEvent(i));

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
