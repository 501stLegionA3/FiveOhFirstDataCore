using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

public class RosterComponentSettings : PageComponentSettingsBase
{
    public bool Scoped { get; set; }

    #region Unscoped Settings
    // These settings are always in effect for the roster display.
    /// <summary>
    /// IF true the roster will only show a list of members rather than
    /// an ordered collection of elements for this roster.
    /// </summary>
    /// <remarks>
    /// When this option is true further details can be displayed about a single
    /// user. Furthermore, this allows further sorting and search options to
    /// be conducted on the roster iteself.
    /// </remarks>
    public bool AllowUserLisiting { get; set; }

    #region User Listing
    /// <summary>
    /// A list of properties to display when the roster is in the user
    /// listing mode.
    /// </summary>
    /// <remarks>
    /// Each item in this list becomes its own column in the user listing.
    /// </remarks>
    public List<DataCoreUserProperty> UserListDisplayedProperties { get; set; } = new();
    #endregion

    #region Default Display
    /// <summary>
    /// A list of properties to display for each user when the roster is in
    /// its default display mode.
    /// </summary>
    /// <remarks>
    /// Each property value here is displayed from left to right by order with
    /// a single space separating the values.
    /// </remarks>
    public List<DataCoreUserProperty> DefaultDisplayedProperties { get; set; } = new();
    #endregion
    #endregion


    #region Scoped Settings
    // These settings are only in effect if the Scoped parameter
    // is set to true - otherwise they are ignored.

    /// <summary>
    /// The level from the top most roster part to start the display at,
    /// for the scoped user.
    /// </summary>
    /// <remarks>
    /// The top level portion of the Roster is 0. If the user does not have
    /// a display avalible because they are either not on the roster, or 
    /// their scope is too high for the level from top provided, no 
    /// roster information will be displayed.
    /// </remarks>
    public int LevelFromTop { get; set; }
    /// <summary>
    /// The max depth of the roster to display along the scoped path.
    /// </summary>
    public int Depth { get; set; }
    #endregion

    /// <summary>
    /// The default roster to display. If left null the roster at index 0
    /// will be displayed.
    /// </summary>
    /// <remarks>
    /// Due to the nature of a database, the item at index 0 can change
    /// depending on how the data is loaded.
    /// </remarks>
    public Guid? DefaultRoster { get; set; }

    /// <summary>
    /// The avalible rosters to display.
    /// </summary>
    /// <remarks>
    /// Multiple rosters can be provided, but scoped configurations should only
    /// provided the main roster to track. Providing different rosters that might
    /// have different configurations can cause unexpected display problems.
    /// </remarks>
    public List<RosterDisplaySettings> AvalibleRosters { get; set; } = new();
}
