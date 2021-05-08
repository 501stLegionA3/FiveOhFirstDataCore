using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(FiveOhFirstDataCore.Areas.Identity.IdentityHostingStartup))]
namespace FiveOhFirstDataCore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}