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
    public Task<ActionResult> AddRosterTreeAsync(string name, Guid? parentTree = null, int position = 0);
    public Task<ActionResult> UpdateRosterTreeAsync(Guid tree, Action<RosterTreeEditModel> action);
    public Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position);
    public Task<ActionResult> RemoveChildRosterAsync(Guid tree, Guid child);
    public Task<ActionResult> RemoveRosterTreeAsync(Guid tree);
    #endregion

    #region Roster Position
    public Task<ActionResult> AddRosterSlotAsync(string name, Guid parentTree, int position);
    public Task<ActionResult> UpdateRosterSlotAsync(Guid slot, Action<RosterSlotEditModel> action);
    public Task<ActionResult> RemoveRosterSlotAsync(Guid slot);
    #endregion

    #region Roster Display Settings
    public Task<ActionResult> AddRosterDisplaySettingsAsync(string name, Guid host);
    public Task<ActionResult> UpdateRosterDisplaySettingsAsync(Guid settings, Action<RosterDisplaySettingsEditModel> action);
    public Task<ActionResult> RemoveRosterDisplaySettingsAsync(Guid settings);
    #endregion

    #region Get Roster Display
    public IAsyncEnumerable<bool> LoadFullRosterTreeAsync(RosterTree tree);
    public Task<ActionResult<RosterTree>> GetRosterTreeForSettingsAsync(Guid settings);
    public Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplays(); 
    #endregion
}
