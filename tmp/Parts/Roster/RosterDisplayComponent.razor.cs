using Microsoft.JSInterop;

namespace ProjectDataCore.Components.Parts.Roster;

[RosterComponent(Name = "Roster")]
public partial class RosterDisplayComponent : RosterDisplayBase
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IJSRuntime JS { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class RosterUserSettingsModel
    {
        public DataCoreUserProperty SelectedParameter { get; set; } = new() { Order = 0 };
        public Guid? SelectedRosterSettings { get; set; }
        public int SelectedRosterPosition { get; set; }
        public bool UserListing { get; set; } = false;
        public List<DataCoreUser> UserListingValues { get; set; } = new();

        public bool SortAscending { get; set; } = true;
        public string Search { get; set; }
        public int SortIndex { get; set; } = 0;
        public DataComparer Comparer = new();

        public List<string>[] DisplayedProperties { get; set; } = Array.Empty<List<string>>();
        private int RegisterIndex { get; set; } = 0;

        public List<SortedSet<string>> SortedProperties { get; set; } = new();

        public void StartUserLoad(int paramSize)
        {
            UserListingValues.Clear();
            RegisterIndex = 0;
            DisplayedProperties = new List<string>[paramSize];

            for (int i = 0; i < DisplayedProperties.Length; i++)
                DisplayedProperties[i] = new();
        }

        public void RegisterBatch(List<DataCoreUserProperty> properties)
        {
            if(properties.Count != DisplayedProperties.Length)
                throw new ArgumentException("The length of properties must be the same as the param size passed during" +
                    " start user load.", nameof(properties));

            for(int i = RegisterIndex; i < UserListingValues.Count; i++)
            {
                var user = UserListingValues[i];

                for(int x = 0; x < properties.Count; x++)
                {
                    string disp = "";
                    if(properties[x].IsStatic)
                    {
                        disp = user.GetStaticProperty(properties[x].PropertyName, properties[x].FormatString);
                    }
                    else
                    {
                        disp = user.GetAssignableProperty(properties[x].PropertyName, properties[x].FormatString);
                    }

                    if(string.IsNullOrWhiteSpace(disp))
                        DisplayedProperties[x].Add("n/a");
                    else
                        DisplayedProperties[x].Add(disp);
                }
            }

            RegisterIndex = UserListingValues.Count;

            foreach (var res in RunSort(properties))
                continue;
        }

        public IEnumerable<bool> RunSort(List<DataCoreUserProperty> properties)
        {
            SortedProperties.Clear();
            Comparer.Ascending = SortAscending;

            for (int i = 0; i < properties.Count; i++)
                SortedProperties.Add(new(Comparer));

            for (int i = 0; i < DisplayedProperties.Length; i++)
            {
                string sortValue = DisplayedProperties[SortIndex][i] ?? "";

                if (sortValue.StartsWith(Search ?? "", StringComparison.OrdinalIgnoreCase))
                {
                    for (int x = 0; x < properties.Count; x++)
                    {
                        var value = DisplayedProperties[x][i] ?? "n/a";

                        SortedProperties[x].Add(value);

                        yield return true;
                    }
                }
            }

            yield return true;
        }

        public class DataComparer : IComparer<string>
        {
            public bool Ascending { get; set; } = true;

            public int Compare(string? x, string? y)
            {
                if (x is null && y is null) return 0;
                if (x is null) return Ascending ? -1 : 1;
                if (y is null) return Ascending ? 1 : -1;

                if (Ascending)
                {
                    var res = x.CompareTo(y);

                    return res == 0 ? 1 : res;
                }
                else
                {
                    var res = y.CompareTo(x);

                    return res == 0 ? 1 : res;
                }
            }
        }
    }

    public RosterUserSettingsModel RosterUserSettings { get; set; } = new();

    public bool CanSwitchRosterSelection { get; set; } = false;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(ComponentData is not null)
        {
            if(ComponentData.AvalibleRosters.Count > 1)
                CanSwitchRosterSelection = true;

            if(ComponentData.DefaultRoster is not null)
            {
                RosterUserSettings.SelectedRosterSettings = ComponentData.DefaultRoster;

                await ReloadRosterTreeAsync();
            }
            else if (ComponentData.AvalibleRosters.Count > 0)
            {
                RosterUserSettings.SelectedRosterSettings = ComponentData.AvalibleRosters[0].Key;

                await ReloadRosterTreeAsync();
            }
        }
    }

    protected void SelectedParameterChanged(int newParameter)
    {
        if (ComponentData is not null)
        {
            if (ComponentData.UserListDisplayedProperties is not null)
            {
                RosterUserSettings.SelectedParameter =
                    ComponentData.UserListDisplayedProperties[newParameter];

                RosterUserSettings.SortIndex = newParameter;

                TriggerSort();
            }
        }
    }

    protected void SearchValueChanged(string newSearch)
    {
        RosterUserSettings.Search = newSearch;

        TriggerSort();
    }

    protected void TriggerSort()
    {
        if (ComponentData is not null)
        {
            _ = Task.Run(async () =>
            {
                int c = 1;
                foreach (var trigger in RosterUserSettings.RunSort(ComponentData.UserListDisplayedProperties))
                {
                    if (c++ >= 5)
                    {
                        await InvokeAsync(StateHasChanged);
                        c = 1;
                    }

                    await InvokeAsync(StateHasChanged);
                }
            });
        }
    }

    protected async Task SelectedRosterChangedAsync(int newPos)
    {
        if(ComponentData is not null)
        {
            RosterUserSettings.SelectedRosterSettings = ComponentData.AvalibleRosters[newPos].Key;

            await ReloadRosterTreeAsync();
        }
    }

    protected async Task ReloadRosterTreeAsync()
    {
        if (RosterUserSettings.SelectedRosterSettings is not null && ComponentData is not null)
        {
            var res = await ModularRosterService.GetRosterTreeForSettingsAsync(
                                RosterUserSettings.SelectedRosterSettings.Value);

            if(res.GetResult(out RosterTree? tree, out var err))
            {
                ActiveTree = tree;

                List<DataCoreUser>? users = null;
                if (RosterUserSettings.UserListing)
                {
                    RosterUserSettings.StartUserLoad(ComponentData.UserListDisplayedProperties.Count);
                    users = RosterUserSettings.UserListingValues;
                }

                var loader = ModularRosterService.LoadFullRosterTreeAsync(tree, users);

                await foreach (var _ in loader)
                {
                    if(RosterUserSettings.UserListing)
                    {
                        RosterUserSettings.RegisterBatch(ComponentData.UserListDisplayedProperties);
                    }

                    StateHasChanged();
                }

                if (RosterUserSettings.UserListing)
                    await ReloadInterop();
            }
            else
            {
                // TODO handle errors.
            }
        }
    }

    protected async Task OnChangeViewMode()
    {
        RosterUserSettings.UserListing = !RosterUserSettings.UserListing;
        
        await ReloadRosterTreeAsync();
    }

    protected void OnChangeSortDirection()
    {
        RosterUserSettings.SortAscending = !RosterUserSettings.SortAscending;

        TriggerSort();
    }

    protected async Task ReloadInterop()
    {
        if (ComponentData is not null)
        {
            var module = await JS.InvokeAsync<IJSObjectReference>("import", "./site.js");

            var data = new string[ComponentData.UserListDisplayedProperties.Count];

            for (int i = 0; i < data.Length; i++)
                data[i] = $"#rt-size-{i}";

            await module.InvokeVoidAsync("callSplit", new object[] { data });
        }
    }
}
