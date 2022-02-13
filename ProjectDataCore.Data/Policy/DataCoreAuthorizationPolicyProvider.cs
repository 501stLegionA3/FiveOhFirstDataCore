using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Services.User;
using ProjectDataCore.Data.Structures.Policy;

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
    private readonly IDataBus _dataBus;

    public DataCoreAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
        IPolicyService policy, IDataBus dataBus)
    {
        _default = new DefaultAuthorizationPolicyProvider(options);
        _policy = policy;
        _dataBus = dataBus;
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => await _default.GetDefaultPolicyAsync();

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => await _default.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var dynamicBuilder = await _policy.GetPolicyBuilderAsync(policyName, false);

        if (dynamicBuilder is not null)
            return dynamicBuilder.WithDataBus(_dataBus).Build();

        return await _default.GetPolicyAsync(policyName);
    }
}
