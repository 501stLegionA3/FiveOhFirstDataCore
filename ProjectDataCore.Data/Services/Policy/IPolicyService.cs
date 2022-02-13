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
}
