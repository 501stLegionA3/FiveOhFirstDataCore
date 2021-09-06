using Microsoft.Extensions.Logging.EventLog;

namespace FiveOhFirstDataCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
#pragma warning disable CA1416 // Validate platform compatibility
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    try
                    {
                        services.Configure<EventLogSettings>(settings =>
                        {
                            settings.SourceName = "501st Data Core";
                        });
                    }
                    catch { /* not on windows */ }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseWindowsService();
#pragma warning restore CA1416 // Validate platform compatibility
    }
}
