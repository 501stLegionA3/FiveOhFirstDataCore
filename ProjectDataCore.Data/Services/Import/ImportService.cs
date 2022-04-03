using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly IAssignableDataService _assignableDataService;

    public ImportService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IInstanceLogger instanceLogger, 
        IServiceProvider serviceProvider, IAssignableDataService assignableDataService)
        => (_dbContextFactory, _instanceLogger, _serviceProvider, _assignableDataService) 
        =  (dbContextFactory, instanceLogger, serviceProvider, assignableDataService);

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
                await using var serviceScope = _serviceProvider.CreateAsyncScope();
                using var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<DataCoreUser>>();

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

                if (user?.UserName == "Administrator")
                {
                    // Skip the admin account.
                    continue;
                }
                // ... then we check if the user is null, and
                // we shound not create new accounts ...
                else if (user is null
                    && !config.CreateNewAccounts)
                {
                    // ... and log a warning for this row ...
                    log.Log($"Skipping row {rowCount} - no user found that matches id/username {row[config.IdentifierColumn]}.", LogLevel.Warning, logScope);
                    continue;
                }
                // ... or if the user is not null, and
                // we have been told to only make new
                // accounts ...
                else if (user is not null
                    && !config.UpdateExistingAccounts)
                {
                    // ... log an info statement and continue ...
                    log.Log($"Skipping row {rowCount} - a user was found and the import is configured to skip existing accounts.", LogLevel.Information, logScope);
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

                // ... if we have an email column ...
                if (config.EmailColumn != -1)
                {
                    // ... then set the emails ...
                    user.Email = row[config.EmailColumn];
                    user.NormalizedEmail = row[config.EmailColumn].Normalize();
                    user.EmailConfirmed = !string.IsNullOrWhiteSpace(user.Email);

                    // ... then log it ...
                    if (user.EmailConfirmed)
                        log.Log($"Updated email for {user.UserName}", LogLevel.Information, logScope);
                    else
                        log.Log($"No email found for {user.UserName}. They need this to receive password resets.", LogLevel.Warning, logScope);
                }

                // ... next up is configuring the assignable values
                // and static properties for the user object ...
                foreach (var bindingPair in config.ValueBindings)
                {
                    // ... throw if we arent supposed to continue ...
                    cancellationToken.ThrowIfCancellationRequested();

                    // ... skip all the unique columns ...
                    if (bindingPair.Key == config.IdentifierColumn
                        || bindingPair.Key == config.EmailColumn
                        || bindingPair.Key == config.PasswordColumn
                        || bindingPair.Key == config.RosterColumn)
                        continue;

                    // ... and grab the binding info ...
                    var binding = bindingPair.Value;

                    using var bindingLog = log.CreateScope($"Started converting {binding.PropertyName}", LogLevel.Information, logScope);

                    // ... create the part holder so we dont have to do this twice ...
                    string[] propertyParts = null;

                    // ... then grab the new value ...

                    // ... if it should auto convert ...
                    if (binding.AutoConvert || binding.IsStatic)
                    {
                        // ... and is static ...
                        if (binding.IsStatic)
                        {
                            // ... then we need to convert it ourselves ...
                            Type? propertyType = userType.GetProperty(binding.PropertyName)?.PropertyType;

                            // ... if the proeprty is null, log it and continue ...
                            if (propertyType is null)
                            {
                                bindingLog.Log($"No property type was found for {binding.PropertyName}", LogLevel.Warning, logScope);
                                continue;
                            }

                            try
                            {
                                // ... otherwise convert the value ...
                                binding.DataValues.Clear();
                                binding.DataValues[row[bindingPair.Key]] = Convert.ChangeType(row[bindingPair.Key], propertyType);
                            }
                            catch (Exception ex)
                            {
                                // ... if the conversion failed, log it and continue ...
                                bindingLog.Log($"Failed to convert {row[bindingPair.Key]} to {propertyType.Name}: {ex.Message}", LogLevel.Error, logScope);
                                continue;
                            }

                            // ... otherwise log the success ...
                            bindingLog.Log($"Got static value: {binding.DataValues}", LogLevel.Information, logScope);
                        }
                        // ... and is dynamic ...
                        else
                        {
                            // ... if there is a multiple value delimiter ...
                            if (config.MultipleValueDelimiter is not null)
                            {
                                // ... break the data down into its parts ...
                                propertyParts = row[bindingPair.Key].Split(config.MultipleValueDelimiter, StringSplitOptions.RemoveEmptyEntries);

                                // ... then trim any whitespace ...
                                for (int i = 0; i < propertyParts.Length; i++)
                                    propertyParts[i] = propertyParts[i].Trim();

                                // ... then save it to the data values ...
                                foreach (var propertyPart in propertyParts)
                                    binding.DataValues[propertyPart] = propertyPart;
                            }
                            else
                            {
                                // ... otherwise add the bulk values to the set ...
                                binding.DataValues[row[bindingPair.Key]] = row[bindingPair.Key];
                            }
                        }
                    }

                    // ... throw if we arent supposed to continue ...
                    cancellationToken.ThrowIfCancellationRequested();

                    // ... if the binding is static ...
                    if (binding.IsStatic)
                    {
                        // ... then we already converted it, so
                        // lets set the value ...
                        userType.GetProperty(binding.PropertyName)?.SetValue(user, binding.DataValues[row[bindingPair.Key]]);
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
                                if(propertyParts is null)
                                {
                                    // ... break the data down into its parts ...
                                    propertyParts = row[bindingPair.Key].Split(config.MultipleValueDelimiter, StringSplitOptions.RemoveEmptyEntries);

                                    // ... then trim any whitespace ...
                                    for (int i = 0; i < propertyParts.Length; i++)
                                        propertyParts[i] = propertyParts[i].Trim();
                                }

                                // ... then clear any exisitng value ...
                                propertyContainer.ClearValue();
                                // ... and save those parts ...
                                foreach(var propertyPart in propertyParts)
                                {
                                    // ... throw if we arent supposed to continue ...
                                    cancellationToken.ThrowIfCancellationRequested();

                                    try
                                    {
                                        if (binding.DataValues.TryGetValue(propertyPart, out var actualBindingValue))
                                        {
                                            propertyContainer.ConvertAndAddValue(actualBindingValue); 
                                            bindingLog.Log($"Saved {actualBindingValue} to {propertyContainer.AssignableConfiguration.PropertyName}", LogLevel.Information, logScope);
                                        }
                                        else
                                        {
                                            bindingLog.Log($"Failed to save {propertyPart} to " +
                                                $"{propertyContainer.AssignableConfiguration.PropertyName}: " +
                                                $"No binding was configured for this value.", LogLevel.Warning, logScope);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // ... then log this as a warning and continue ...
                                        bindingLog.Log($"Failed to save {propertyPart} to " +
                                            $"{propertyContainer.AssignableConfiguration.PropertyName}: {ex.Message}", 
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
                                if (binding.DataValues.TryGetValue(row[bindingPair.Key], out var actualBindingValue))
                                {
                                    propertyContainer.ConvertAndAddValue(actualBindingValue);
                                    bindingLog.Log($"Saved {actualBindingValue} to {propertyContainer.AssignableConfiguration.PropertyName}", LogLevel.Information, logScope);
                                }
                                else
                                {
                                    bindingLog.Log($"Failed to save {row[bindingPair.Key]} to " +
                                        $"{propertyContainer.AssignableConfiguration.PropertyName}: " +
                                        $"No binding was configured for this value.", LogLevel.Warning, logScope);
                                }
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

                // ... throw if we arent supposed to continue ...
                cancellationToken.ThrowIfCancellationRequested();

                // ... now that we have saved all the updated information
                // its time to work on the roster values ...
                if(config.RosterColumn != -1)
                {
                    using var rosterLog = log.CreateScope("Starting Roster Assignments", LogLevel.Information, logScope);

                    // ... lets get the binding pair for the roster ...
                    if(config.ValueBindings.TryGetValue(config.RosterColumn, out var bindingPair))
                    {
                        // ... the for each part of the row data ...
                        var dataRaw = row[config.RosterColumn];
                        var data = dataRaw.Split(config.MultipleValueDelimiter).ToList(x => x.Trim());

                        // ... then get all roster tree data so
                        // we dont assign two people to the same
                        // spot on accident ...
                        var trees = await _dbContext.RosterTrees
                            .Include(x => x.RosterPositions)
                            .ToListAsync();

                        // ... throw if we arent supposed to continue ...
                        cancellationToken.ThrowIfCancellationRequested();

                        // ... and each roster import conditional ...
                        int conditionalCount = 0;
                        foreach(var conditional in bindingPair.RosterImportConditionals)
                        {
                            conditionalCount++;

                            // ... throw if we arent supposed to continue ...
                            cancellationToken.ThrowIfCancellationRequested();

                            // ... make sure we have all information we need
                            // to use this conditional ...
                            if (conditional.RosterTree == default
                                || conditional.SlotRange.Count <= 0)
                                continue;

                            // ... check the validity of the condition ...
                            if (conditional.Resolve(data))
                            {
                                // ... and if it is valid, get the roster object ...
                                var tree = trees.FirstOrDefault(x => x.Key == conditional.RosterTree);

                                // ... if the roster tree is null ...
                                if(tree is null)
                                {
                                    // ... log and continue to the next comparision ...
                                    rosterLog.Log($"Could not find Roster Tree {conditional.RosterTree}", LogLevel.Warning, logScope);
                                    continue;
                                }

                                // ... otherwise log and assign values ...
                                rosterLog.Log($"Found roster tree {tree.Name} to assign {user.UserName} to.", LogLevel.Information, logScope);

                                // ... then for each range pair in the 
                                // configured conditional ...
                                bool found = false;
                                foreach(var range in conditional.SlotRange)
                                {
                                    // ... throw if we arent supposed to continue ...
                                    cancellationToken.ThrowIfCancellationRequested();

                                    for (int x = range.Start.Value; x < range.End.Value; x++)
                                    {
                                        // ... throw if we arent supposed to continue ...
                                        cancellationToken.ThrowIfCancellationRequested();

                                        // ... find a roster position in the tree that matcehs the index
                                        // value ...
                                        var position = tree.RosterPositions.FirstOrDefault(y => y.Order.Order == x);
                                        // ... if it exists and is not occupied ...
                                        if(position is not null
                                            && position.OccupiedById is null)
                                        {
                                            // ... then assign it to this user  ...
                                            position.OccupiedById = user.Id;
                                            found = true;

                                            // ... then log and exit the loops ...
                                            rosterLog.Log($"Added {user.UserName} to {position.Name}", LogLevel.Information, logScope);
                                            break;
                                        }
                                    }

                                    // ... additional exit statement ...
                                    if (found)
                                        break;
                                }

                                // ... if nothing was found at all, log that ...
                                if(!found)
                                {
                                    rosterLog.Log($"Failed to find a roster position for {user.UserName} in conditional {conditionalCount}.", LogLevel.Information, logScope);
                                }
                            }
                        }
                    }
                }
            }

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
        while ((line = await sr.ReadLineAsync()) is not null)
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

            // ... if we have a header row, and we are on it ...
            if (config.HasHeaderRow && lineNumber == 0)
            {
                // ... then save the values to the header config ...
                foreach (var p in parts)
                    config.HeaderValues.Add(p);
            }
            // ... otherwise, this is not the header ...
            else
            {
                // ... if we dont have a header row ...
                if(!config.HasHeaderRow && lineNumber == 0)
                {
                    // ... populate the values with default data ...
                    for (int i = 0; i < cols; i++)
                        config.HeaderValues.Add($"Row {i}");
                }

                // ... then trim any whitespace ...
                for(int i = 0; i < cols; i++)
                    parts[i] = parts[i].Trim();

                // ... then save the data to the config ...
                config.DataRows.Add(parts);

                // ... now for each column ...
                for (int i = 0; i < cols; i++)
                {
                    // ... get the local parts array ...
                    List<string> localParts = new(); ;
                    if (config.MultipleValueDelimiter is not null)
                    {
                        // ... and add all entries if we have a multiple value delimiter ...
                        localParts.AddRange(parts[i].Split(config.MultipleValueDelimiter, StringSplitOptions.RemoveEmptyEntries));
                        for(int x = 0; x < localParts.Count; x++)
                            localParts[x] = localParts[x].Trim();
                    }
                    else
                    {
                        // ... otherwise just add the value ...
                        localParts.Add(parts[i]);
                    }

                    // ... and for the values in that column ...
                    config.UniqueValues.AddOrUpdate(i,
                        // ... Add the value to the unqiue values list ...
                        (key) =>
                        {
                            return new(localParts);
                        },
                        // ... or update the value to the uniqe values list ...
                        (key, value) =>
                        {
                            // We utilize a HashSet here, so only
                            // new items will be added.
                            foreach (var lp in localParts)
                                value.Add(lp);
                            return value;
                        });
                }

                // ... then continue until the stream has been completely read.
            }
        }

        // ... then tell the caller the operation is done.
        return new(true, null);

        // Stream closing should be handled by the user, and not in this method.
        // Only the StreamReader is closed.
    }
}
