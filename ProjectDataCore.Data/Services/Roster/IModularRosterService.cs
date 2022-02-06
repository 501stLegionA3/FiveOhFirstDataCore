using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Page.Components;
using ProjectDataCore.Data.Structures.Result;
using ProjectDataCore.Data.Structures.Roster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services;

/// <summary>
/// A service for maintaining the module roster.
/// </summary>
public interface IModularRosterService
{
    #region Roster Tree
    public Task<ActionResult<Guid>> AddRosterTreeAsync(string name, Guid? parentTree = null, int position = 0);
    public Task<ActionResult> UpdateRosterTreeAsync(Guid tree, Action<RosterTreeEditModel> action);
    public Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position);
    public Task<ActionResult> RemoveChildRosterAsync(Guid tree, Guid child);
    public Task<ActionResult> RemoveRosterTreeAsync(Guid tree);
    #endregion

    #region Roster Position
    public Task<ActionResult> AddRosterSlotAsync(string name, Guid parentTree, int position);
    public Task<ActionResult> UpdateRosterSlotAsync(Guid slot, Action<RosterSlotEditModel> action);
    public Task<ActionResult> RemoveRosterSlotAsync(Guid slot);
    public Task<ActionResult> LoadExistingSlotsAsync(DataCoreUser activeUser);
    #endregion

    #region Roster Display Settings
    public Task<ActionResult> AddRosterDisplaySettingsAsync(string name, Guid host);
    public Task<ActionResult> LoadEditableDisplaysAsync(EditableComponentSettings componentData);
    public Task<ActionResult> UpdateRosterDisplaySettingsAsync(Guid settings, Action<RosterDisplaySettingsEditModel> action);
    public Task<ActionResult> RemoveRosterDisplaySettingsAsync(Guid settings);
    #endregion

    #region Get Roster Display
    public IAsyncEnumerable<bool> LoadFullRosterTreeAsync(RosterTree tree, List<DataCoreUser>? userList = null);
    public Task<ActionResult<RosterTree>> GetRosterTreeForSettingsAsync(Guid settings);
    public Task<ActionResult<RosterTree>> GetRosterTreeByIdAsync(Guid tree);
    public Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplaysAsync();
    public Task<ActionResult<List<RosterTree>>> GetOrphanedRosterTreesAsync();
    public Task<ActionResult<List<RosterTree>>> GetTopLevelRosterTreesAsync();
    public Task<ActionResult<List<RosterTree>>> GetAllRosterTreesAsync();
    #endregion
}
