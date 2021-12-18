namespace ProjectDataCore.Components.Parts.Roster;

[RosterComponent(Name = "Roster")]
public partial class RosterDisplayComponent : RosterDisplayBase
{
    public class RosterUserSettingsModel
    {
        public string Search { get; set; }
        public DataCoreUserProperty SelectedParameter { get; set; } = new() { Order = 0 };
        public Guid? SelectedRosterSettings { get; set; }
        public int SelectedRosterPosition { get; set; }
        public bool UserListing { get; set; } = false;
        public bool SortAscending { get; set; } = true;
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
            }
            else if (ComponentData.AvalibleRosters.Count > 0)
            {
                RosterUserSettings.SelectedRosterSettings = ComponentData.AvalibleRosters[0].Key;
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
        if (RosterUserSettings.SelectedRosterSettings is not null)
        {
            var res = await ModularRosterService.GetRosterTreeForSettingsAsync(
                                RosterUserSettings.SelectedRosterSettings.Value);

            if(res.GetResult(out RosterTree? tree, out var err))
            {
                ActiveTree = tree;

                StateHasChanged();
            }
            else
            {
                // TODO handle errors.
            }
        }
    }

    protected void OnChangeViewMode()
        => RosterUserSettings.UserListing = !RosterUserSettings.UserListing;

    protected void OnChangeSortDirection()
        => RosterUserSettings.SortAscending = !RosterUserSettings.SortAscending;
}
