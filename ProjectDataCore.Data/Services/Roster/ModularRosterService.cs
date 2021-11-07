using ProjectDataCore.Data.Structures.Result;
using ProjectDataCore.Data.Structures.Roster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services;

public class ModularRosterService : IModularRosterService
{
    #region Roster Tree
    public async Task<ActionResult> AddRosterTreeAsync(string name, Guid parentTree)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> UpdateRosterTreeAsync(Guid tree, string? newName = null, Guid? newParent = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveChildRosterAsync(Guid tree, Guid child)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> UpdateChildRosterPositionAsync(Guid tree, Guid child, int newPosition)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveRosterTreeAsync(Guid tree)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Roster Position
    public async Task<ActionResult> AddRosterSlotAsync(string name, Guid parentTree)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> UpdateRosterSlotAsync(Guid slot, string? newName = null, Guid? newParent = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> UpdateRosterSlotPositionAsync(Guid parentTree, Guid slot, int newPosition)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveRosterSlotAsync(Guid slot)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Roster Display Settings
    public async Task<ActionResult> AddRosterDisplaySettingsAsync(string name, bool whitelisted)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> UpdateRosterDisplaySettingsAsync(Guid settings, bool? whitelisted = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> AddTreeToDisplaySettingsAsync(Guid settings, Guid tree)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveTreeFromDisplaySettingsAsync(Guid settings, Guid tree)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveRosterDisplaySettingsAsync(Guid settings)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Get Roster Display
    public IAsyncEnumerable<RosterTree> GetFullRosterAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<RosterTree> GetRosterTreeForSettingsAsync(Guid settings)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplays()
    {
        throw new NotImplementedException();
    }
    #endregion
}
