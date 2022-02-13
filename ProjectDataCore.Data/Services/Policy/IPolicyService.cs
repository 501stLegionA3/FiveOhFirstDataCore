using ProjectDataCore.Data.Structures.Model.Policy;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Policy;
public interface IPolicyService
{
    public Task InitalizeAsync();
    public Task<DynamicAuthroizationPolicyBuilder?> GetPolicyBuilderAsync(string component, bool forceReload);

    #region Policy Editing
    public Task<ActionResult> CreatePolicyAsync(DynamicAuthorizationPolicy policy);
    public Task<ActionResult> UpdatePolicyAsync(Guid key, Action<DynamicAuthorizationPolicyEditModel> action);
    public Task<ActionResult> DeletePolicyAsync(Guid key);
    #endregion
}
