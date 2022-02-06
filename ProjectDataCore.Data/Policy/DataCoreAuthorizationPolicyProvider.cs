using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using ProjectDataCore.Data.Services.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Policy;
public class DataCoreAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _default;
    private readonly IPolicyService _policy;

    public DataCoreAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
        IPolicyService policy)
    {
        _default = new DefaultAuthorizationPolicyProvider(options);
        _policy = policy;
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
     => await _default.GetDefaultPolicyAsync();

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
     => await _default.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        //var dynamicPolicy = _policy.GetPolicyBuilder(policyName);

        throw new NotImplementedException();
    }
}
