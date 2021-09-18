using FiveOhFirstDataCore.Core.Structures.Policy;

using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Auth;

public class DynamicPolicyAuthorizationPolicyBuilder : AuthorizationPolicyBuilder
{
    private readonly DynamicPolicy _policy;

    public DynamicPolicyAuthorizationPolicyBuilder(DynamicPolicy policy)
        => _policy = policy;

    public void RequireAssertion(Func<AuthorizationHandlerContext, DynamicPolicy, bool> handler)
            => RequireAssertion(ctx => handler.Invoke(ctx, _policy));

    public void RequireAssertion(Func<AuthorizationHandlerContext, DynamicPolicy, Task<bool>> handler)
        => RequireAssertion(ctx => handler.Invoke(ctx, _policy));
}
