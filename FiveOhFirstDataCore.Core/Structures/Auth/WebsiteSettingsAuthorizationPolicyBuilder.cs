using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Authorization;

namespace FiveOhFirstDataCore.Data.Structures.Auth
{
    public class WebsiteSettingsAuthorizationPolicyBuilder : AuthorizationPolicyBuilder
    {
        private readonly IWebsiteSettingsService _settings;

        public WebsiteSettingsAuthorizationPolicyBuilder(IWebsiteSettingsService settings)
        {
            _settings = settings;
        }

        public void RequireAssertion(Func<AuthorizationHandlerContext, IWebsiteSettingsService, bool> handler)
            => RequireAssertion(ctx => handler.Invoke(ctx, _settings));

        public void RequireAssertion(Func<AuthorizationHandlerContext, IWebsiteSettingsService, Task<bool>> handler)
            => RequireAssertion(ctx => handler.Invoke(ctx, _settings));
    }
}
