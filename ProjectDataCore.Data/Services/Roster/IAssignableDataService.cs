using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Roster;

/// <summary>
/// A service to handle the creation and management of Assignable data vlaues.
/// </summary>
public interface IAssignableDataService
{
    #region Configuration Editing
    public Task<ActionResult> AddNewAssignableConfiguration(BaseAssignableConfiguration config);
    public Task<ActionResult> UpdateAssignableConfiguration<T>(Guid configKey, Action<AssignableConfigurationEditModel<T>> update);
    public Task<ActionResult> DeleteAssignableConfiguration(Guid configKey);
    #endregion

    #region Value Editing
    public Task<ActionResult> UpdateAssignableValue<T>(Guid user, Guid config, List<T> value);
    #endregion
}
