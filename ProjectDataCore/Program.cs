using ProjectDataCore;
using Microsoft.Extensions.Logging.EventLog;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
#pragma warning disable CA1416 // Validate platform compatibility
        => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile(Path.Join("Config", "website_config.json"));
            })
            .ConfigureServices((hostContext, services) =>
            {
                try
                {
                    services.Configure<EventLogSettings>(settings =>
                    {
                        settings.SourceName = "Project Data Core";
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
