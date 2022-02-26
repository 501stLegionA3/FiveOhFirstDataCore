using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Selectors;
public partial class MultiListSelectSelector<TValue> : ComponentBase
{
    /// <summary>
    /// An action to trigger when the value changes.
    /// </summary>
    [Parameter]
    public Action? OnValueChanged { get; set; }

    /// <summary>
    /// The currently selected items.
    /// </summary>
    [Parameter]
    public List<TValue> SelectedItems { get; set; } = new();

    /// <summary>
    /// A list of values to select from.
    /// </summary>
    [Parameter]
    public List<TValue> Values { get; set; } = new();

    /// <summary>
    /// A list of values to display insated of the value in <see cref="Values"/>.
    /// </summary>
    [Parameter]
    public List<string> DisplayValues { get; set; } = new();
    [Parameter]
    public string DisplayName { get; set; } = "Value Selector";

    public string SearchRaw { get; set; } = "";
    public List<TValue> SuggestedItems { get; set; } = new();

    public List<int> SelectedDisplays { get; set; } = new();

    protected void OnSearchUpdated(string? value)
    {
        SearchRaw = value ?? "";

        if(DisplayValues.Count > 0)
        {
            var sugDisp = DisplayValues
                .Where(x => x.StartsWith(SearchRaw, StringComparison.OrdinalIgnoreCase))
                .Take(5);

            SuggestedItems.Clear();
            foreach(var s in sugDisp)
            {
                try
                {
                    SuggestedItems.Add(Values[DisplayValues.IndexOf(s)]);
                }
                catch
                {
                    // Do nothing.
                }
            }
        }
        else
        {
            SuggestedItems = Values
                .Where(x => x?.ToString()?.StartsWith(SearchRaw, StringComparison.OrdinalIgnoreCase) ?? false)
                .Take(5)
                .ToList();
        }
    }

    protected void OnAddSelection(TValue user)
    {
        SelectedItems.Add(user);
        SearchRaw = "";

        OnValueChanged?.Invoke();
        StateHasChanged();
    }

    protected void OnRemoveSelection(TValue user)
    {
        SelectedItems.Remove(user);

        OnValueChanged?.Invoke();
        StateHasChanged();
    }

    protected string GetDisplayValue(TValue item)
    {
        if(DisplayValues.Count > 0)
        {
            try
            {
                return DisplayValues[Values.IndexOf(item)];
            }
            catch
            {
                return "err: no display";
            }
        }
        else
        {
            return item?.ToString() ?? "err: no display";
        }
    }
}
