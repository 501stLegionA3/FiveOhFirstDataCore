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
        public string Search { get; set; }
        public DataCoreUserProperty SelectedParameter { get; set; } = new() { Order = 0 };
        public Guid? SelectedRosterSettings { get; set; }
        public int SelectedRosterPosition { get; set; }
        public bool UserListing { get; set; } = false;
        public List<DataCoreUser> UserListingValues { get; set; } = new();
        public bool SortAscending { get; set; } = true;

        public List<string>[] DisplayedProperties { get; set; } = Array.Empty<List<string>>();
        private int RegisterIndex { get; set; } = 0;

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
                    DisplayedProperties[x].Add(disp);
                }
            }

            RegisterIndex = UserListingValues.Count;
        }

        public IEnumerable<bool> RunSort(int paramIndex, string value)
        {
            throw new NotImplementedException();
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
        if(ComponentData is not null)
            if(ComponentData.UserListDisplayedProperties is not null)
                RosterUserSettings.SelectedParameter =
                    ComponentData.UserListDisplayedProperties[newParameter];


    }

    protected void SearchValueChanged(string newSearch)
    {
        RosterUserSettings.Search = newSearch;


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
        => RosterUserSettings.SortAscending = !RosterUserSettings.SortAscending;

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
