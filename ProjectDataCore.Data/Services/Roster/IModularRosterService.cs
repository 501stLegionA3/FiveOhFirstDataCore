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
    public Task<ActionResult> AddRosterTreeAsync(string name, Guid parentTree);
    public Task<ActionResult> UpdateRosterTreeAsync(Guid tree, string? newName = null, Guid? newParent = null);
    public Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position);
    public Task<ActionResult> RemoveChildRosterAsync(Guid tree, Guid child);
    public Task<ActionResult> UpdateChildRosterPositionAsync(Guid tree, Guid child, int newPosition);
    public Task<ActionResult> RemoveRosterTreeAsync(Guid tree);
    #endregion

    #region Roster Position
    public Task<ActionResult> AddRosterSlotAsync(string name, Guid parentTree);
    public Task<ActionResult> UpdateRosterSlotAsync(Guid slot, string? newName = null, Guid? newParent = null);
    public Task<ActionResult> UpdateRosterSlotPositionAsync(Guid parentTree, Guid slot, int newPosition);
    public Task<ActionResult> RemoveRosterSlotAsync(Guid slot);
    #endregion

    #region Roster Display Settings
    public Task<ActionResult> AddRosterDisplaySettingsAsync(string name, bool whitelisted);
    public Task<ActionResult> UpdateRosterDisplaySettingsAsync(Guid settings, bool? whitelisted = null);
    public Task<ActionResult> AddTreeToDisplaySettingsAsync(Guid settings, Guid tree);
    public Task<ActionResult> RemoveTreeFromDisplaySettingsAsync(Guid settings, Guid tree);
    public Task<ActionResult> RemoveRosterDisplaySettingsAsync(Guid settings);
    #endregion

    #region Get Roster Display
    public IAsyncEnumerable<RosterTree> GetFullRosterAsync();
    public IAsyncEnumerable<RosterTree> GetRosterTreeForSettingsAsync(Guid settings);
    public Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplays(); 
    #endregion
}
