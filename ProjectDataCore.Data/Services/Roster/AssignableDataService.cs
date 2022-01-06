﻿using ProjectDataCore.Data.Structures.Assignable;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Roster;

public class AssignableDataService : IAssignableDataService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public AssignableDataService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => _dbContextFactory = dbContextFactory;

    public async Task<ActionResult> AddNewAssignableConfiguration(BaseAssignableConfiguration config)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the assignable configuration attribute for this config ...
        var configType = config.GetType();
        var attr = configType.GetCustomAttributes<AssignableConfigurationAttribute>()
            .FirstOrDefault();
        // ... and ensure it exists ...
        if (attr is null)
            return new(false, new List<string>() { "No assignable configuration attribute was found for the provided config." });

        // ... then save the new config to the database ...
        var res = await _dbContext.AddAsync(config);
        await _dbContext.SaveChangesAsync();

        List<BaseAssignableValue> valueObjects = new();

        // ... then restore the config object to include the new key ...
        await res.ReloadAsync();

        // ... then for all users ...
        var users = _dbContext.Users.AsAsyncEnumerable();
        await foreach(var user in users)
        {
            // ... create a new instance of the assignable value ...
            if (Activator.CreateInstance(attr.Configures) is BaseAssignableValue assignable)
            {
                // ... set the links ...
                assignable.AssignableConfigurationId = config.Key;
                assignable.ForUserId = user.Id;
                // ... then add it to the list of objects ...
                valueObjects.Add(assignable);
            }
            else
            {
                // ... if an error occours, immedietly exit ...
                return new(false, new List<string>() { "Unable to create an assignable value from the provided configuration." });
            }
        }
        // ... then for each value object ...
        foreach (var i in valueObjects)
            // ... add it to the database ...
            await _dbContext.AddAsync(i);

        // ... and save changes ...
        await _dbContext.SaveChangesAsync();

        // ... then reurn a success.
        return new(true, null);
    }

    public async Task<ActionResult> DeleteAssignableConfiguration(Guid configKey)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // ... find the object ...
        var obj = await _dbContext.FindAsync<BaseAssignableConfiguration>(configKey);

        if (obj is null)
            return new(false, new List<string>() { "Configuration for the provided key was not found." });

        // ... then delete it ...
        _dbContext.Remove(obj);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdateAssignableConfiguration<T>(Guid configKey, Action<AssignableConfigurationEditModel<T>> update)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // ... find the object ...
        var obj = await _dbContext.FindAsync<BaseAssignableConfiguration>(configKey);

        if (obj is null)
            return new(false, new List<string>() { "Configuration for the provided key was not found." });

        // ... then save the update information ...
        AssignableConfigurationEditModel<T> editModel = new();
        update.Invoke(editModel);

        if (editModel.AllowMultiple is not null)
            obj.AllowMultiple = editModel.AllowMultiple.Value;

        if(editModel.AllowedInput is not null)
            obj.AllowedInput = editModel.AllowedInput.Value;

        if (!string.IsNullOrWhiteSpace(editModel.PropertyName))
            obj.PropertyName = editModel.PropertyName;

        if (editModel.AllowedValues is not null)
        {
            // ... then find the configuration type ...
            switch (obj)
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                case DateTimeValueAssignableConfiguration c:
                    c.AvalibleValues = editModel.AllowedValues.ToList(x => (DateTime)Convert.ChangeType(x, typeof(DateTime)));
                    break;
                case DateOnlyValueAssignableConfiguration c:
                    c.AvalibleValues = editModel.AllowedValues.ToList(x => (DateOnly)Convert.ChangeType(x, typeof(DateOnly)));
                    break;
                case TimeOnlyValueAssignableConfiguration c:
                    c.AvalibleValues = editModel.AllowedValues.ToList(x => (TimeOnly)Convert.ChangeType(x, typeof(TimeOnly)));
                    break;


                case IntegerValueAssignableConfiguration c:
                    c.AvalibleValues = editModel.AllowedValues.ToList(x => (int)Convert.ChangeType(x, typeof(int)));
                    break;
                case DoubleValueAssignableConfiguration c:
                    c.AvalibleValues = editModel.AllowedValues.ToList(x => (double)Convert.ChangeType(x, typeof(double)));
                    break;


                case StringValueAssignableConfiguration c:
                    c.AvalibleValues = editModel.AllowedValues.ToList(x => (string?)Convert.ChangeType(x, typeof(string)) ?? "");
                    break;
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
        }

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdateAssignableValue<T>(Guid user, Guid config, List<T> value)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var assignable = await _dbContext.AssignableValues
            .Where(x => x.ForUserId == user)
            .Where(x => x.AssignableConfigurationId == config)
            .Include(x => x.AssignableConfiguration)
            .FirstOrDefaultAsync();

        if (assignable is null)
            return new(false, new List<string>() { "No assignable value was found." });

        switch (assignable)
        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
            case DateTimeAssignableValue c:
                if (c.SetValue.GetType() != value.GetType())
                    return new(false, new List<string>() { "Assignable value type missmatch." });

                c.SetValue = value.ToList(x => (DateTime)Convert.ChangeType(x, typeof(DateTime)));
                break;
            case DateOnlyAssignableValue c:
                if (c.SetValue.GetType() != value.GetType())
                    return new(false, new List<string>() { "Assignable value type missmatch." });

                c.SetValue = value.ToList(x => (DateOnly)Convert.ChangeType(x, typeof(DateOnly)));
                break;
            case TimeOnlyAssignableValue c:
                if (c.SetValue.GetType() != value.GetType())
                    return new(false, new List<string>() { "Assignable value type missmatch." });

                c.SetValue = value.ToList(x => (TimeOnly)Convert.ChangeType(x, typeof(TimeOnly)));
                break;


            case IntegerAssignableValue c:
                if (c.SetValue.GetType() != value.GetType())
                    return new(false, new List<string>() { "Assignable value type missmatch." });

                c.SetValue = value.ToList(x => (int)Convert.ChangeType(x, typeof(int)));
                break;
            case DoubleAssignableValue c:
                if (c.SetValue.GetType() != value.GetType())
                    return new(false, new List<string>() { "Assignable value type missmatch." });

                c.SetValue = value.ToList(x => (double)Convert.ChangeType(x, typeof(double)));
                break;


            case StringAssignableValue c:
                if (c.SetValue.GetType() != value.GetType())
                    return new(false, new List<string>() { "Assignable value type missmatch." });

                c.SetValue = value.ToList(x => (string?)Convert.ChangeType(x, typeof(string)) ?? "");
                break;
#pragma warning restore CS8605 // Unboxing a possibly null value.
        }

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }
}
