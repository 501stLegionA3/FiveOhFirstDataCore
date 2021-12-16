namespace ProjectDataCore.Components.Parts.Roster;

public partial class RosterDisplayComponent : RosterDisplayBase
{
    public class RosterUserSettingsModel
    {
        public string Search { get; set; }
        public DataCoreUserProperty SelectedParameter { get; set; }
        public Guid? SelectedRoster { get; set; }
        public bool UserListing { get; set; } = false;
        public bool SortAscending { get; set; } = true;
    }

    public RosterUserSettingsModel RosterUserSettings { get; set; } = new();
    public RosterComponentSettingsEditModel RosterDisplaySettings { get; set; } = new();

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
                RosterUserSettings.SelectedRoster = ComponentData.DefaultRoster;
            }
            else if (ComponentData.AvalibleRosters.Count > 0)
            {
                RosterUserSettings.SelectedRoster = ComponentData.AvalibleRosters[0].Key;
            }


        }
    }

    protected void SelectedParameterChanged(DataCoreUserProperty newParameter)
    {
        RosterUserSettings.SelectedParameter = newParameter;


    }

    protected void SearchValueChanged(string newSearch)
    {
        RosterUserSettings.Search = newSearch;


    }

    protected async Task SelectedRosterChangedAsync(Guid? newRoster)
    {

    }

    protected void OnChangeViewMode()
    {

    }

    protected void OnChangeSortDirection()
    {

    }
}
