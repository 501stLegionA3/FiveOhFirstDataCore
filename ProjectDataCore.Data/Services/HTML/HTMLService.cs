using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.HTML;
public class HTMLService : IHTMLService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IConfiguration _configuration;

    private readonly SemaphoreSlim _cssUpdateLock = new(1, 1);
    private bool installed = false;

    public HTMLService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IWebHostEnvironment hostingEnvironment,
        IConfiguration configuration)
    {
        _dbContextFactory = dbContextFactory;
        _hostingEnvironment = hostingEnvironment;
        _configuration = configuration;
    }

    public async Task<ActionResult> UpdateCustomSiteCSSAsync()
    {
        StringBuilder builder = new();
        await using (var _dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            // Get all displays ...
            var displays = _dbContext.DisplayComponentSettings
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsAsyncEnumerable();

            // ... then take their raw html contents ...
            await foreach (var item in displays)
                builder.AppendLine(item.Raw);
        }
        // ... then toss the DB context, we are done with it ...

        // ... and wait for the update lock to be free ...
        if(await _cssUpdateLock.WaitAsync(TimeSpan.FromMinutes(1)))
        {
            try
            {
                // ... then get the root path ...
                string rootPath = _hostingEnvironment.WebRootPath;
                rootPath = Path.Combine(rootPath, "css", "custom");

                // ... write the HTML to a file ...
                await using (var fs = new FileStream(Path.Combine(rootPath, "custom.html"), FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using var sw = new StreamWriter(fs);

                    await sw.WriteAsync(builder);
                }

                string sep = _configuration["Config:CustomCSS:CommandSeparator"];

                // ... then build the post css process (requires npm) ...
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = _configuration["Config:CustomCSS:CommandLineTool"],
                        WorkingDirectory = rootPath,
                        Arguments = $"/C{(installed ? "" : $" npm i {sep}")} npm run css"
                    }
                };

                List<string> errors = new();
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data is not null)
                        errors.Add(e.Data);
                };

                process.Start();
                await process.WaitForExitAsync();

                if (errors.Count < 0)
                {
                    installed = true;
                    return new(true, null);
                }

                errors.Insert(0, "Failed to run PostCSS");
                return new(false, errors);
            }
            finally
            {
                _ = _cssUpdateLock.Release();
            }
        }
        else
        {
            // ... the system was too busy or took too long on
            // a previous action ...
            return new(false, new List<string> { "Failed to receive start signal", "Custom CSS builder was used by another" +
                " user and failed to start." });
        }
    }
}
