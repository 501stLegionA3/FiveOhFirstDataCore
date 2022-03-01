using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Util.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Import;

public class ImportService : IImportService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ImportService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => _dbContextFactory = dbContextFactory;

    public async Task<ActionResult> BulkUpdateUsersAsync(DataImportConfiguration config, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new(false, new List<string>() { "Cancellation was requested." });

        try
        {
            await using var _dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            List<DataCoreUser> users = new();

            foreach(var row in config.DataRows)
            {

            }
        }
        catch (OperationCanceledException)
        {
            return new(false, new List<string>() { "Cancellation was requested." });
        }
    }

    public async Task<ActionResult> GetCSVUniqueValuesAsync(Stream dataStream, DataImportConfiguration config)
    {
        using StreamReader sr = new(dataStream);

        // Time to read the data.
        string? line = null;
        int lineNumber = -1;
        int cols = -1;
        while((line = await sr.ReadLineAsync()) is not null)
        {
            lineNumber++;

            // Lets break our line apart by delimiter ...
            // (we are not removing empty entries so the data is valid)
            var parts = line.Split(config.StandardDelimiter);

            // ... then validate the column counts match ...
            if (cols < 0)
            {
                cols = parts.Length;
            }
            else if (parts.Length != cols)
            {
                config.DataRows.Clear();

                return new(false,
                    new List<string>() {
                        $"Failed to parse data: line {lineNumber} has a different number of columns than the first line."
                    });
            }

            // ... then save the data to the config ...
            config.DataRows.Add(parts);

            // ... now for each column ...
            for (int i = 0; i < parts.Length; i++)
            {
                // ... and for the values in that column ...
                config.UniqueValues.AddOrUpdate(i, 
                    // ... Add the value to the unqiue values list ...
                    (key) => {
                        return new() { parts[i] };
                    },
                    // ... or update the value to the uniqe values list ...
                    (key, value) =>
                    {
                        // We utilize a HashSet here, so only
                        // new items will be added.
                        value.Add(parts[i]);
                        return value;
                    });
            }

            // ... then continue until the stream has been completely read.
        }

        // ... then tell the caller the operation is done.
        return new(true, null);

        // Stream closing should be handled by the user, and not in this method.
        // Only the StreamReader is closed.
    }
}
