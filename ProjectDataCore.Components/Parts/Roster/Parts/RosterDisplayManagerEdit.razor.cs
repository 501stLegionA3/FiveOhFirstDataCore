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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

        public async Task ULPE_AddProperty(Func<Task> RefreshCaller)
        {
            UserListingProperties.Add(UserListingProperties.Count, new()
            {
                PropertyName = ULPE_PropertyName,
                IsStatic = ULPE_Static,
                Order = UserListingProperties.Count
            });

            ULPE_PropertyName = "";
            ULPE_Static = false;

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

        public async Task RDPE_AddProperty(Func<Task> RefreshCaller)
        {
            RosterDisplayProperties.Add(RosterDisplayProperties.Count, new()
            {
                PropertyName = RDPE_PropertyName,
                IsStatic = RDPE_Static,
                Order = RosterDisplayProperties.Count
            });

            RDPE_PropertyName = "";
            RDPE_Static = false;

            await RefreshCaller.Invoke();
        }
        #endregion
        
        public List<RosterDisplaySettings> AvalibleRosters { get; set; } = new();

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

        }
    }

    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }

    [Parameter]
    public RosterComponentSettings? CurrentSettings { get; set; }

    public RosterComponentModel? RosterComponentSettings { get; set; }
    public List<RosterDisplaySettings> AvalibleRosters { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(CurrentSettings is not null)
            RosterComponentSettings = new(CurrentSettings);
    }

    protected async Task OnRefreshRequested()
    {
        await InvokeAsync(StateHasChanged);
    }
}
