using DSharpPlus.Entities;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Misc;
public partial class StaticDiscordRoleInputList
{
    [Parameter]
    public List<ulong>? Items { get; set; } = new();
    [Parameter]
    public bool MakeListNullOnEmpty { get; set; } = true;
    [Parameter]
    public IReadOnlyList<DiscordRole> Source { get; set; } = Array.Empty<DiscordRole>();

    public string NewValueFilter { get; set; } = "";
    public ulong NewValue { get; set; } = 0;

    public void AddValue()
    {
        if (NewValue > 0)
        {
            if (Items is null)
                Items = new();
            Items.Add(NewValue);
        }

        NewValue = 0;
    }

    public void RemoveValue(int index)
    {
        if (Items is not null)
        {
            Items.RemoveAt(index);
            if (Items.Count <= 0)
                Items = null;

            StateHasChanged();
        }
    }
}
