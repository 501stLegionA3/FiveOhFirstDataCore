using Microsoft.AspNetCore.Authorization;

using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Policy;
public class DynamicAuthroizationPolicyBuilder : AuthorizationPolicyBuilder
{
    public DynamicAuthorizationPolicy Policy { get; set; }
    public IDataBus DataBus { get; set; }

    public void RequireAssertion(Func<AuthorizationHandlerContext, DynamicAuthorizationPolicy, IDataBus, bool> handler)
        => RequireAssertion(ctx => handler.Invoke(ctx, Policy, DataBus));

    public void RequireAssertion(Func<AuthorizationHandlerContext, DynamicAuthorizationPolicy, IDataBus, Task<bool>> handler)
        => RequireAssertion(ctx => handler.Invoke(ctx, Policy, DataBus));

    public DynamicAuthroizationPolicyBuilder WithDataBus(IDataBus dataBus)
    {
        DataBus = dataBus;
        return this;
    }

    public DynamicAuthroizationPolicyBuilder WithPolicy(DynamicAuthorizationPolicy policy)
    {
        Policy = policy;
        return this;
    }
}
