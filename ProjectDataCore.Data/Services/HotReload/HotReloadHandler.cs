using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.HotReload;
public class HotReloadHandler
{
    private static IWebHostEnvironment HostEnvironment { get; set; }
    private static IConfiguration Configuration { get; set; }

    public HotReloadHandler(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
	{
        HostEnvironment = hostEnvironment;
        Configuration = configuration;
	}

    private static void UpdateApplication(Type[]? updatedTypes)
    {
        _ = Task.Run(async () =>
        {
            if (HostEnvironment is null || Configuration is null)
                return;

            // ... then get the root path ...
            string contentRoot = Path.GetFullPath(Path.Combine(HostEnvironment.ContentRootPath, ".."));

#if DEBUG
            string cfgname = "Debug";
#else
            string cfgname = "Release";
#endif

            var targetFramework = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(TargetFrameworkAttribute))
                .SingleOrDefault() as TargetFrameworkAttribute;

            if (targetFramework is null)
                return;

            var framework = $"net{targetFramework.FrameworkName.Split('=').ElementAtOrDefault(1)?[1..]}";

            string sep = Configuration["Config:CustomCSS:CommandSeparator"];

            List<string> cmds = new()
            {
                $@"npx postcss {Path.Combine(contentRoot, @$"ProjectDataCore\wwwroot\css\app.css")} -o {Path.Combine(contentRoot, $@"ProjectDataCore\wwwroot\css\app.min.css")}",
                $@"npx postcss {Path.Combine(contentRoot, $@"ProjectDataCore.Components\obj\{cfgname}\{framework}\scopedcss\bundle\ProjectDataCore.Components.styles.css")} -o {Path.Combine(contentRoot, $@"ProjectDataCore.Components\wwwroot\css\scoped\project.min.css")}",
                $@"npx postcss {Path.Combine(contentRoot, $@"ProjectDataCore.Components\obj\{cfgname}\{framework}\scopedcss\projectbundle\ProjectDataCore.Components.bundle.scp.css")} -o {Path.Combine(contentRoot, $@"ProjectDataCore.Components\wwwroot\css\scoped\bundle.min.css")}",
                $@"npx postcss {Path.Combine(contentRoot, @$"ProjectDataCore\obj\{cfgname}\{framework}\scopedcss\bundle\ProjectDataCore.styles.css")} -o {Path.Combine(contentRoot, $@"ProjectDataCore\wwwroot\css\scoped\project.min.css")}",
                $@"npx postcss {Path.Combine(contentRoot, @$"ProjectDataCore\obj\{cfgname}\{framework}\scopedcss\projectbundle\ProjectDataCore.bundle.scp.css")} -o {Path.Combine(contentRoot, $@"ProjectDataCore\wwwroot\css\scoped\bundle.min.css")}",
#if DEBUG
                $@"npx webpack --config ../webpack.config.debug.js",
#else
                $@"npx webpack --config ../webpack.config.js",
#endif
            };

            // ... then build the post css process (requires npm) ...
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Configuration["Config:CustomCSS:CommandLineTool"],
                    WorkingDirectory = Path.Combine(contentRoot, "ProjectDataCore"),
                    Arguments = $"/C {string.Join($" {sep} ", cmds)}",
                }
            };

            Console.WriteLine("START HOTRELOAD SCRIPTS ...");

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                    Console.WriteLine(e.Data);
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                    Console.WriteLine(e.Data);
            };

            process.Start();
            await process.WaitForExitAsync();

            Console.WriteLine("... END HOTRELOAD SCRIPTS");
        });
    }
}
