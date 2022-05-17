using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Roster.Parts;
public partial class RosterDisplayManagerEdit
{
#pragma warning disable CS8618 // Injections are not null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
    [Inject]
    public IJSRuntime JS { get; set; }
    [Inject]
    public IModularRosterService ModularRosterService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Handles the edit form for the Roster Display component.
    /// </summary>
    public class RosterComponentModel
    {
        public bool Scoped { get; set; }
        public int LevelFromTop { get; set; }
        public int Depth { get; set; }

        public bool AllowUserListing { get; set; }
        public SortedList<int, DataCoreUserProperty> UserListingProperties { get; set; }

        #region User Listing Property Edit
        public string ULPE_PropertyName { get; set; } = "";
        public bool ULPE_Static { get; set; } = false;
        public Type[] ULPE_AllowedStatic { get; } = new Type[]
        {
            typeof(int),
            typeof(string),
            typeof(double),
            typeof(ulong),
            typeof(Guid),
            typeof(DateTime)
        };

        public void ULPE_ProeprtyNameChanged(string newName)
            => ULPE_PropertyName = newName;

        public void ULPE_StaticChanged(bool newStatic)
            => ULPE_Static = newStatic;

        // See code comments for the RDPE methods - they are the exact same
        // except they modify a different list.
        public async Task ULPE_AddProperty(Func<Task> RefreshCaller)
        {
            UserListingProperties.Add(UserListingProperties.Count, new()
            {
                PropertyName = ULPE_PropertyName,
                IsStatic = ULPE_Static,
                Order = UserListingProperties.Count
            });

            ULPE_PropertyName = "";

            await RefreshCaller.Invoke();
        }

        public async Task ULPE_RemoveProperty(int pos, Func<Task> RefreshCaller)
        {
            UserListingProperties.RemoveAt(pos);

            SortedList<int, DataCoreUserProperty> list = new();
            var enumer = UserListingProperties.GetEnumerator();
            enumer.MoveNext();

            for (int i = 0; i < UserListingProperties.Count; i++)
            {
                var cur = enumer.Current;
                cur.Value.Order = i;

                list.Add(i, cur.Value);

                enumer.MoveNext();
            }

            UserListingProperties = list;
            await RefreshCaller.Invoke();
        }

        public async Task ULPE_MoveItem(int start, int moveBy, Func<Task> RefreshCaller)
        {
            // Already as far to the left as possible.
            var otherIndex = start + moveBy;
            if (otherIndex < 0 || otherIndex > UserListingProperties.Count - 1)
                return;

            var prev = UserListingProperties[otherIndex];
            UserListingProperties[otherIndex] = UserListingProperties[start];
            UserListingProperties[start] = prev;

            UserListingProperties[otherIndex].Order = otherIndex;
            UserListingProperties[start].Order = start;

            await RefreshCaller.Invoke();
        }
        #endregion

        public SortedList<int, DataCoreUserProperty> RosterDisplayProperties { get; set; }

        #region Roster Display Property Edit
        public string RDPE_PropertyName { get; set; } = "";
        public bool RDPE_Static { get; set; } = false;
        public Type[] RDPE_AllowedStatic { get; } = new Type[]
        {
            typeof(int),
            typeof(string)
        };

        public void RDPE_ProeprtyNameChanged(string newName)
            => RDPE_PropertyName = newName;

        public void RDPE_StaticChanged(bool newStatic)
            => RDPE_Static = newStatic;

        /// <summary>
        /// Adds a new property for the Roster Display Property list.
        /// </summary>
        /// <param name="RefreshCaller">The pages refresh caller.</param>
        /// <returns>A task for this action</returns>
        public async Task RDPE_AddProperty(Func<Task> RefreshCaller)
        {
            RosterDisplayProperties.Add(RosterDisplayProperties.Count, new()
            {
                PropertyName = RDPE_PropertyName,
                IsStatic = RDPE_Static,
                Order = RosterDisplayProperties.Count
            });

            RDPE_PropertyName = "";

            await RefreshCaller.Invoke();
        }

        /// <summary>
        /// Removes a property from the Roster Display Property list.
        /// </summary>
        /// <param name="pos">The position to remove.</param>
        /// <param name="RefreshCaller">The pages refresh caller.</param>
        /// <returns>A task for this action.</returns>
        public async Task RDPE_RemoveProperty(int pos, Func<Task> RefreshCaller)
        {
            RosterDisplayProperties.RemoveAt(pos);

            SortedList<int, DataCoreUserProperty> list = new();
            var enumer = RosterDisplayProperties.GetEnumerator();
            enumer.MoveNext();

            for (int i = 0; i < RosterDisplayProperties.Count; i++)
            {
                var cur = enumer.Current;
                cur.Value.Order = i;

                list.Add(i, cur.Value);

                enumer.MoveNext();
            }

            RosterDisplayProperties = list;
            await RefreshCaller.Invoke();
        }

        /// <summary>
        /// Moves an item in the Roster Display Property list.
        /// </summary>
        /// <param name="start">The starting elemnts position.</param>
        /// <param name="moveBy">The relative index to siwtch the item with. (-1 move left by one, 1 move right by one).</param>
        /// <param name="RefreshCaller">The pages refresh caller.</param>
        /// <returns>A task for this action.</returns>
        public async Task RDPE_MoveItem(int start, int moveBy, Func<Task> RefreshCaller)
        {
            // Already as far to the left as possible.
            var otherIndex = start + moveBy;
            if (otherIndex < 0 || otherIndex > RosterDisplayProperties.Count - 1)
                return;

            var prev = RosterDisplayProperties[otherIndex];
            RosterDisplayProperties[otherIndex] = RosterDisplayProperties[start];
            RosterDisplayProperties[start] = prev;

            RosterDisplayProperties[otherIndex].Order = otherIndex;
            RosterDisplayProperties[start].Order = start;

            await RefreshCaller.Invoke();
        }
        #endregion

        public List<RosterDisplaySettings> AvalibleRosters { get; set; } = new();
        #region Avalible Roster Edits
        /// <summary>
        /// A list of rosters to select from.
        /// </summary>
        public List<RosterDisplaySettings> RosterSelection { get; set; } = new();
        public int? SelectedRoster { get; set; }
        public void SelectedRosterChanged(int? index)
            => SelectedRoster = index;

        /// <summary>
        /// Adds a roster to the avalible rosters.
        /// </summary>
        /// <param name="RefreshCaller">The pages refresh caller.</param>
        /// <returns>A task representing this action.</returns>
        public async Task AddRoster(Func<Task> RefreshCaller)
        {
            if(SelectedRoster is not null)
                AvalibleRosters.Add(RosterSelection[SelectedRoster.Value]);

            await RefreshCaller.Invoke();
        }

        /// <summary>
        /// Removes a roster from the avalible rosters.
        /// </summary>
        /// <param name="pos">The position to remove a roster from.</param>
        /// <param name="RefreshCaller">The pages refresh caller.</param>
        /// <returns>A task representing this action.</returns>
        public async Task RemoveRoster(int pos, Func<Task> RefreshCaller)
        {
            AvalibleRosters.RemoveAt(pos);

            await RefreshCaller.Invoke();
        }
        #endregion

        public RosterComponentModel(RosterComponentSettings settings)
        {
            Scoped = settings.Scoped;
            LevelFromTop = settings.LevelFromTop;
            Depth = settings.Depth;
            AllowUserListing = settings.AllowUserLisiting;

            UserListingProperties = new();
            foreach (var i in settings.UserListDisplayedProperties)
                UserListingProperties.Add(i.Order, i);

            RosterDisplayProperties = new();
            foreach (var i in settings.DefaultDisplayedProperties)
                RosterDisplayProperties.Add(i.Order, i);

            AvalibleRosters.AddRange(settings.AvalibleRosters); 
        }
    }

    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }
    [CascadingParameter(Name = "RefreshRequest")]
    public Func<Task>? CallRefreshRequest { get; set; }
    [Parameter]
    public RosterComponentSettings? CurrentSettings { get; set; }

    public RosterComponentModel? RosterComponentSettings { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // Load the roster settings.
        if(CurrentSettings is not null)
            RosterComponentSettings = new(CurrentSettings);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await ReloadRosterDisplays();
        }
    }

    protected async Task ReloadRosterDisplays()
    {
        // Get all avalible rosters to display ...
        var res = await ModularRosterService.GetAvalibleRosterDisplaysAsync();

        if (res.GetResult(out var displays, out var err))
        {
            // ... and add them as options.
            if(RosterComponentSettings is not null)
                RosterComponentSettings.RosterSelection = displays;
        }
        else
        {
            // TODO do something with the error if it occours.
        }

        StateHasChanged();
    }

    protected async Task OnRefreshRequested()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected async Task SaveChangesAsync()
    {
        if (CurrentSettings is not null && RosterComponentSettings is not null)
        {
            // Take the settings from Roster Component Settings and send them
            // to the Page Edit Service to update the database
            var res = await PageEditService.UpdateRosterComponentAsync(CurrentSettings.Key, (x) =>
            {
                x.AllowUserListing = RosterComponentSettings.AllowUserListing;
                x.UserListDisplayedProperties = RosterComponentSettings.UserListingProperties
                                                    .Select(x => x.Value).ToList();
                
                x.Scoped = RosterComponentSettings.Scoped;
                x.LevelFromTop = RosterComponentSettings.LevelFromTop;
                x.Depth = RosterComponentSettings.Depth;

                x.DefaultDisplayedProperties = RosterComponentSettings.RosterDisplayProperties
                                                    .Select(x => x.Value).ToList();

                x.AvalibleRosters = RosterComponentSettings.AvalibleRosters;
            });

            // TODO handle errors.

            // Then refresh the entire page just
            // to make sure all settings were saved
            // properly.
            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }

    protected void ResetChanges()
    {
        if(CurrentSettings is not null)
            RosterComponentSettings = new(CurrentSettings);

        StateHasChanged();
    }
}
