using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Services.Logging;
using ProjectDataCore.Data.Services.Roster;
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
    private readonly IInstanceLogger _instanceLogger;
    private readonly UserManager<DataCoreUser> _userManager;
    private readonly IAssignableDataService _assignableDataService;

    public ImportService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IInstanceLogger instanceLogger, 
        UserManager<DataCoreUser> userManager, IAssignableDataService assignableDataService)
        => (_dbContextFactory, _instanceLogger, _userManager, _assignableDataService) 
        =  (dbContextFactory, instanceLogger, userManager, assignableDataService);

    public async Task<ActionResult> BulkUpdateUsersAsync(DataImportConfiguration config, CancellationToken cancellationToken = default, Guid logScope = default)
    {
        try
        {
            // Lets start crunching data ...
            cancellationToken.ThrowIfCancellationRequested();

            // ... by checking to ensure we have a column to identify
            // the indiviudals in this data set ...
            if(config.IdentifierColumn == -1)
            {
                var msg = "No identifier column configured.";
                _instanceLogger.Log(msg, LogLevel.Critical, logScope);
                return new(false, new List<string>() { msg });
            }

            // ... then get the type for data core user to
            // user later on ...
            Type userType = typeof(DataCoreUser);

            // ... then creating our main database context ...
            await using var _dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            List<DataCoreUser> users = new();

            // ... then for each row ...
            int rowCount = 0;
            int maxRows = config.DataRows.Count;
            DataImportBinding? idBinding = null;
            foreach(var row in config.DataRows)
            {
                rowCount++;

                // ... throw if we arent supposed to continue ...
                cancellationToken.ThrowIfCancellationRequested();

                // ... log the row information ...
                using var log = _instanceLogger.CreateScope($"Processing row {rowCount} of {maxRows}", LogLevel.Information, logScope);

                // ... then attempt to find the user for this row ...
                DataCoreUser? user = null;
                if(idBinding is not null 
                    || config.ValueBindings.TryGetValue(config.IdentifierColumn, out idBinding))
                {
                    // ... throw if we arent supposed to continue ...
                    cancellationToken.ThrowIfCancellationRequested();

                    // ... by pulling the id column ....
                    var id = row[config.IdentifierColumn];

                    // ... and checking the user ID ...
                    if(idBinding.IsUserIdIdentifier)
                    {
                        // ... if we find it, its a value, if not
                        // user stays as null ...
                        var userId = Guid.Parse(id);
                        user = await _dbContext.Users
                            .Where(x => x.Id == userId)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(cancellationToken);
                    }
                    // ... or checking the username ...
                    else if (idBinding.IsUsernameIdentifier)
                    {
                        // ... if we find it, its a value, if not
                        // user stays null ...
                        user = await _dbContext.Users
                            .Where(x => x.UserName == id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(cancellationToken);
                    }
                    // ... or throwing an error if neither the ID
                    // or username was configured to be checked ...
                    else
                    {
                        var msg = $"ID column is not configured with an ID or Username.";
                        _instanceLogger.Log(msg, LogLevel.Critical, logScope);
                        return new(false, new List<string>() { msg });
                    }
                }
                // ... or throwing an error if no ID column was
                // provided in the config ...
                else
                {
                    var msg = $"Failed to find an ID binding column.";
                    _instanceLogger.Log(msg, LogLevel.Critical, logScope);
                    return new(false, new List<string>() { msg });
                }

                // ... throw if we arent supposed to continue ...
                cancellationToken.ThrowIfCancellationRequested();

                // ... then we check if the user is null, and
                // we shound not create new accounts ...
                if (user is null
                    && !config.CreateNewAccounts)
                {
                    // ... and log a warning for this row ...
                    log.Log($"Skipping row {rowCount} - no user found that matches id/username {row[config.IdentifierColumn]}.", LogLevel.Warning, logScope);
                    continue;
                }
                // ... or if the user is null, the id is a username,
                // and we should create new accounts ...
                else if (user is null 
                    && config.CreateNewAccounts
                    && idBinding.IsUsernameIdentifier)
                {
                    // ... then log the creation of a new account ...
                    log.Log($"Creating a new account for {row[config.IdentifierColumn]}.", LogLevel.Information, logScope);
                        
                    // ... make a new user with a random password ...
                    var res = await _userManager.CreateAsync(new()
                    {
                        UserName = row[config.IdentifierColumn],
                        // ... get the email only if it a column for it exists ...
                        Email = config.EmailColumn != -1 ? row[config.EmailColumn] : ""
                    }, Guid.NewGuid().ToString());

                    // ... then make sure we dont have any errors ...
                    if(!res.Succeeded)
                    {
                        foreach(var msg in res.Errors)
                            log.Log(msg.Description, LogLevel.Critical, logScope);

                        // ... after logging the errors, go to the next line ...
                        continue;
                    }

                    // ... throw if we arent supposed to continue ...
                    cancellationToken.ThrowIfCancellationRequested();

                    // ... then try and get the new user again ...
                    user = await _userManager.FindByNameAsync(row[config.IdentifierColumn]);

                    // ... if the user does snot exist ...
                    if(user is null)
                    {
                        // ... then log and continue ...
                        log.Log($"Could not find a user after creating it for {row[config.IdentifierColumn]}.", LogLevel.Warning, logScope);
                        continue;
                    }
                }
                else
                {
                    // ... otherwise log the user we found ...
                    log.Log($"Found user {user!.UserName} [{user.Id}].", LogLevel.Information, logScope);
                }

                // ... throw if we arent supposed to continue ...
                cancellationToken.ThrowIfCancellationRequested();

                // ... our user is currently untracked, so we are going to
                // ensure all assignable values are on the object ...
                var assignRes = await _assignableDataService.EnsureAssignableValuesAsync(user);

                if(!assignRes.GetResult(out var err))
                {
                    log.Log(err.FirstOrDefault() ?? "An unknown error occoured ensuring assignable values.", LogLevel.Warning, logScope);
                    continue;
                }

                // ... now that the values are ensured,
                // lets grab the actual user object
                // with tracking ...
                user = await _dbContext.Users
                    .Where(x => x.Id == user.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if(user is null)
                {
                    // ... we should really not hit this point
                    // but if we do, log it and continue ...
                    log.Log($"No user was found when re-grabbing a user for tracking on row {rowCount}", LogLevel.Warning, logScope);
                    continue;
                }

                // ... throw if we arent supposed to continue ...
                cancellationToken.ThrowIfCancellationRequested();

                // ... if we have a password column ...
                if (config.PasswordColumn != -1)
                {
                    // ... then set the password hash to the hash we got from
                    // the import ...
                    user.PasswordHash = row[config.PasswordColumn];

                    // ... then log it ...
                    log.Log($"Updated password for {user.UserName}", LogLevel.Information, logScope);
                }

                // ... next up is configuring the assignable values
                // and static properties for the user object ...
                foreach(var bindingPair in config.ValueBindings)
                {
                    // ... throw if we arent supposed to continue ...
                    cancellationToken.ThrowIfCancellationRequested();

                    // ... skip all the unique columns ...
                    if (bindingPair.Key == config.IdentifierColumn
                        || bindingPair.Key == config.EmailColumn
                        || bindingPair.Key == config.PasswordColumn)
                        continue;

                    // ... and grab the binding info ...
                    var binding = bindingPair.Value;

                    using var bindingLog = log.CreateScope($"Started converting {binding.PropertyName}", LogLevel.Information, logScope);

                    // ... then grab the new value ...

                    // ... if it should auto convert
                    // and is static ...
                    if (binding.AutoConvert && binding.IsStatic)
                    {
                        // ... then we need to convert it ourselves ...
                        Type? propertyType = userType.GetProperty(binding.PropertyName)?.PropertyType;

                        // ... if the proeprty is null, log it and continue ...
                        if(propertyType is null)
                        {
                            bindingLog.Log($"No property type was found for {binding.PropertyName}", LogLevel.Warning, logScope);
                            continue;
                        }

                        try
                        {
                            // ... otherwise convert the value ...
                            binding.DataValue = Convert.ChangeType(row[bindingPair.Key], propertyType);
                        }
                        catch (Exception ex)
                        {
                            // ... if the conversion failed, log it and continue ...
                            bindingLog.Log($"Failed to convert {row[bindingPair.Key]} to {propertyType.Name}: {ex.Message}", LogLevel.Error, logScope);
                            continue;
                        }

                        // ... otherwise log the success ...
                        bindingLog.Log($"Got static value: {binding.DataValue}", LogLevel.Information, logScope);
                    }

                    // ... throw if we arent supposed to continue ...
                    cancellationToken.ThrowIfCancellationRequested();

                    // ... if the binding is static ...
                    if (binding.IsStatic)
                    {
                        // ... then we already converted it, so 
                        // lets set the value ...
                        userType.GetProperty(binding.PropertyName)?.SetValue(user, binding.DataValue);
                    }
                    else
                    {
                        // ... otherwise we need to get the proeprty container ...
                        var propertyContainer = user.GetAssignablePropertyContainer(binding.PropertyName);

                        // ... if the container is null ...
                        if(propertyContainer is null)
                        {
                            // ... then log this as a warning and continue ...
                            bindingLog.Log($"No assignable property container was found for {binding.PropertyName}", LogLevel.Warning, logScope);
                            continue;
                        }

                        // ... throw if we arent supposed to continue ...
                        cancellationToken.ThrowIfCancellationRequested();

                        // ... if there are multiple values allowed ...
                        if (propertyContainer.AssignableConfiguration.AllowMultiple)
                        {
                            // ... and a custom delimiter is set ...
                            if(config.MultipleValueDelimiter is not null)
                            {
                                // ... break the data down into its parts ...
                                var propertyParts = row[bindingPair.Key].Split(config.MultipleValueDelimiter, StringSplitOptions.RemoveEmptyEntries);
                                // ... then clear any exisitng value ...
                                propertyContainer.ClearValue();
                                // ... and save those parts ...
                                foreach(var propertyPart in propertyParts)
                                {
                                    // ... throw if we arent supposed to continue ...
                                    cancellationToken.ThrowIfCancellationRequested();

                                    try
                                    {
                                        propertyContainer.ConvertAndAddValue(propertyPart);
                                    }
                                    catch (Exception ex)
                                    {
                                        // ... then log this as a warning and continue ...
                                        bindingLog.Log($"Failed to save {propertyPart} to {propertyContainer.AssignableConfiguration.PropertyName}: {ex.Message}", 
                                            LogLevel.Warning, logScope);
                                        continue;
                                    }
                                }

                                // ... and now we have saved a multiple input value ...
                            }
                            else
                            {
                                // ... otherwise log this as an error and continue ...
                                bindingLog.Log($"No custom delimiter set for multiple value properties!", LogLevel.Error, logScope);
                                continue;
                            }
                        }
                        else
                        {
                            // ... if the value is a single value,
                            // then all we have to do is set the value ...
                            try
                            {
                                propertyContainer.ConvertAndReplaceValue(row[bindingPair.Key], 0);
                            }
                            catch (Exception ex)
                            {
                                // ... something went wrong, so log this as a warning and continue ...
                                bindingLog.Log($"Failed to save {row[bindingPair.Key]} to {propertyContainer.AssignableConfiguration.PropertyName}: {ex.Message}",
                                    LogLevel.Warning, logScope);
                                continue;
                            }

                            // ... and now we have saved a single input value ...
                        }
                    }
                }

                // ... now that we have saved all the updated information ...

                // ... throw if we arent supposed to continue ...
                cancellationToken.ThrowIfCancellationRequested();

                // ... then save to the datbase ...

                try
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    var msg = $"Failed to save to database: {ex.Message}";
                    _instanceLogger.Log(msg, LogLevel.Critical, logScope);
                    return new(false, new List<string>() { msg });
                }
            }
        }
        catch (OperationCanceledException)
        {
            var msg = $"Cancellation was requested.";
            _instanceLogger.Log(msg, LogLevel.Critical, logScope);
            return new(false, new List<string>() { msg });
        }

        // ... if we made it this far, that means we are done!
        return new(true, null);
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
