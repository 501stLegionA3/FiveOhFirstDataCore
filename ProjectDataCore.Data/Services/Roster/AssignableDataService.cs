using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Assignable;
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

        // ... then use the attribute to set the type name ...
        config.TypeName = attr.Name;

        config.NormalizedPropertyName = config.PropertyName.Normalize();

        var curCount = await _dbContext.AssignableConfigurations
            .Where(x => x.NormalizedPropertyName == config.NormalizedPropertyName)
            .CountAsync();

        if (curCount > 0)
            return new(false, new List<string>() { "A property of this name alredy exists." });

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

    public async Task<ActionResult<List<BaseAssignableConfiguration>>> GetAllAssignableConfigurationsAsync()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var data = await _dbContext.AssignableConfigurations.ToListAsync();

        return new(true, null, data);
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
                    c.AllowedValues = editModel.AllowedValues.ToList(x => (DateTime)Convert.ChangeType(x, typeof(DateTime)));
                    break;
                case DateOnlyValueAssignableConfiguration c:
                    c.AllowedValues = editModel.AllowedValues.ToList(x => (DateOnly)Convert.ChangeType(x, typeof(DateOnly)));
                    break;
                case TimeOnlyValueAssignableConfiguration c:
                    c.AllowedValues = editModel.AllowedValues.ToList(x => (TimeOnly)Convert.ChangeType(x, typeof(TimeOnly)));
                    break;


                case IntegerValueAssignableConfiguration c:
                    c.AllowedValues = editModel.AllowedValues.ToList(x => (int)Convert.ChangeType(x, typeof(int)));
                    break;
                case DoubleValueAssignableConfiguration c:
                    c.AllowedValues = editModel.AllowedValues.ToList(x => (double)Convert.ChangeType(x, typeof(double)));
                    break;


                case StringValueAssignableConfiguration c:
                    c.AllowedValues = editModel.AllowedValues.ToList(x => (string?)Convert.ChangeType(x, typeof(string)) ?? "");
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

    public async Task<ActionResult> EnsureAssignableValuesAsync(DataCoreUser user)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        if (user.Id == default)
            return new(false, new List<string>() { "User must be a valid database object." });

        // Reload the assignable values.
        await _dbContext.Attach(user).Collection(e => e.AssignableValues).LoadAsync();

        var keyMap = new HashSet<Guid>();
        foreach (var i in user.AssignableValues)
            keyMap.Add(i.AssignableConfigurationId);

        // Validate that there are no values missing ...
        var missing = await _dbContext.AssignableConfigurations
            .Where(x => !keyMap.Contains(x.Key))
            .Where(x => x.AssignableType == BaseAssignableConfiguration.InternalAssignableType.UserProperty)
            .ToListAsync();

        // ... then for each value that is missing ...
        foreach(var val in missing)
        {
            // ... get the attribute information ...
            var attr = val.GetType().GetCustomAttributes<AssignableConfigurationAttribute>()
                .FirstOrDefault();

            if (attr is null)
                continue;

            // ... create the assignable value ...
            if (Activator.CreateInstance(attr.Configures) is BaseAssignableValue assignable)
            {
                // ... set the links ...
                assignable.AssignableConfigurationId = val.Key;
                assignable.ForUserId = user.Id;
                // ... then add it to the list of objects ...
                await _dbContext.AddAsync(assignable);
            }
            else
            {
                // ... if an error occours, immedietly exit ...
                return new(false, new List<string>() { "Unable to create an assignable value from the provided configuration." });
            }
        }

        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(user).ReloadAsync();

        return new(true, null);
    }

    public async Task<ActionResult<DataCoreUser>> GetMockUserWithAssignablesAsync()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Validate that there are no values missing ...
        var missing = await _dbContext.AssignableConfigurations
            .Where(x => x.AssignableType == BaseAssignableConfiguration.InternalAssignableType.UserProperty)
            .ToListAsync();

        DataCoreUser mock = new();
        // ... then for each value that is missing ...
        foreach (var val in missing)
        {
            // ... get the attribute information ...
            var attr = val.GetType().GetCustomAttributes<AssignableConfigurationAttribute>()
                .FirstOrDefault();

            if (attr is null)
                continue;

            // ... create the assignable value ...
            if (Activator.CreateInstance(attr.Configures) is BaseAssignableValue assignable)
            {
                assignable.AssignableConfiguration = val;
                // ... set the links ...
                mock.AssignableValues.Add(assignable);
            }
            else
            {
                // ... if an error occours, immedietly exit ...
                return new(false, new List<string>() { "Unable to create an assignable value from the provided configuration." });
            }
        }

        return new(true, null, mock);
    }
}
