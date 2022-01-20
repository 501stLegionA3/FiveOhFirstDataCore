using ProjectDataCore.Data.Services.Routing;
using ProjectDataCore.Data.Structures.Model.Page;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Attributes;
using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Page;

public class PageEditService : IPageEditService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IRoutingService _routingService;
    private readonly IScopedUserService _scopedUserService;

    public PageEditService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IRoutingService routingService, IScopedUserService scopedUserService)
        => (_dbContextFactory, _routingService, _scopedUserService) = (dbContextFactory, routingService, scopedUserService);

    #region Page Actions
    public async Task<ActionResult> CreateNewPageAsync(string name, string route)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        // create a new page to add ...
        var obj = new CustomPageSettings()
        {
            Name = name,
            Route = route
        };
        // ... add it ...
        await _dbContext.AddAsync(obj);
        try
        {
            // ... then save.
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            // ... if there was a violation of the unique constraint, then
            // return the error.
            return new(false, new List<string>() { "Route name is already in use.", ex.Message });
        }
    }

    public async Task<ActionResult> DeletePageAsync(Guid page)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var obj = await _dbContext.FindAsync<CustomPageSettings>(page);

        if (obj is null)
            return new(false, new List<string>() { "No page for the provided ID was found." });

        // Load the settings using the currently tracked object
        // so we can do a proper cascade delete ...
        await foreach (var _ in _routingService.LoadPageSettingsAsync(obj)) { }

        _dbContext.Remove(obj);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<List<CustomPageSettings>> GetAllPagesAsync()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await _dbContext.CustomPageSettings.ToListAsync();
    }

    public async Task<ActionResult<CustomPageSettings>> GetPageSettingsAsync(Guid page)
    {
        // Code here is modified from the IRoutingService load page to load a 
        // full page settings object.

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        // find the settings object...
        var obj = await _dbContext.FindAsync<CustomPageSettings>(page);

        if (obj is null)
            return new(false, new List<string>() { "No settings object was found"}, null);

        // ... get the DB data for it ...
        var dbDat = _dbContext.Entry(obj);
        // ... then load the base layout ...
        await dbDat.Reference(e => e.Layout).LoadAsync();
        // ... if the layout is not null ...
        if (obj.Layout is not null)
        {
            // ... then the base layout to the level queue ...
            Queue<LayoutComponentSettings> level = new();
            level.Enqueue(obj.Layout);

            // ... and while we have level data ...
            while (level.TryDequeue(out var levelItem))
            {
                // ... attach the level item ...
                var layoutObj = _dbContext.Attach(levelItem);
                // ... and load its direct children ...
                var children = layoutObj.Collection(x => x.ChildComponents).Query()
                    .Include(x => x.ParentLayout)
                    .AsAsyncEnumerable();

                // ... then for each child ...
                await foreach (var child in children)
                {
                    // ... if the child is a layout component, enqueue it.
                    if (child is LayoutComponentSettings childLayout)
                        level.Enqueue(childLayout);
                }
            }
        }

        // ... and return the fully loaded settings object.
        return new(true, null, obj);
    }

    public async Task<ActionResult> SetPageLayoutAsync(Guid page, Type layout)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var pageData = await _dbContext.CustomPageSettings
            .Include(x => x.Layout)
            .Where(x => x.Key == page)
            .FirstOrDefaultAsync();

        if (pageData is null)
            return new(false, new List<string>() { "No page settings object was found for the provided ID." });

        // Remove old layout data if it exists.
        if(pageData.Layout is not null)
        {
            _dbContext.Remove(pageData.Layout);
            await _dbContext.SaveChangesAsync();
        }

        // Validate layout name
        var name = layout.FullName;

        if (name is null)
            return new(false, new List<string>() { $"No qualified type name was able to be found for the provided {nameof(layout)}." });
        
        // Create and save new layout of page data.
        var layoutData = new LayoutComponentSettings()
        {
            QualifiedTypeName = name
        };

        pageData.Layout = layoutData;

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdatePageAsync(Guid page, Action<CustomPageSettingsEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var pageData = await _dbContext.FindAsync<CustomPageSettings>(page);

        if (pageData is null)
            return new(false, new List<string>() { "No page settings object was found for the provided ID." });

        // Run update based on the action values that were passed.
        CustomPageSettingsEditModel update = new();
        action.Invoke(update);

        if(update.Name is not null)
            pageData.Name = update.Name;

        if(update.Route is not null)
            pageData.Route = update.Route;

        try
        {
            // attempt a save...
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            // ... if there was a violation of the unique constraint, then
            // return the error.
            return new(false, new List<string>() { "Route name is already in use.", ex.Message });
        }
    }
    #endregion

    #region Global Component Actions
    public async Task<ActionResult> UpdateDisplayNameAsync(Guid key, string componentName)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var comp = await _dbContext.FindAsync<PageComponentSettingsBase>(key);
        if (comp is null)
            return new(false, new List<string>() { "No component settings found for the provided key. " });

        comp.DisplayName = componentName;

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult<List<PageComponentSettingsBase>>> GetAvalibleScopesAsync()
    {
        var scopes = _scopedUserService.GetAllActiveScopes();

        if (scopes.Count <= 0)
            return new(true, null, new());

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        List<PageComponentSettingsBase> scopeData = new();
        foreach(var scope in scopes)
        {
            var data = await _dbContext.FindAsync<PageComponentSettingsBase>(scope);
            if(data is not null)
                scopeData.Add(data);
        }

        return new(true, null, scopeData);
    }
    #endregion

    #region Layout Component Actions
    public async Task<ActionResult> SetLayoutChildAsync(Guid layout, Type component, int position)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the current layout data.
        var layoutData = await _dbContext.LayoutComponentSettings
            .Where(x => x.Key == layout)
            .Include(x => x.ChildComponents)
            .FirstOrDefaultAsync();

        if (layoutData is null)
            return new(false, new List<string>() { "No page settings object was found for the provided ID." });

        // Remove any old component data.
        PageComponentSettingsBase? oldComponent;
        if ((oldComponent = layoutData.ChildComponents.FirstOrDefault(x => x.Order == position)) is not null)
        {
            layoutData.ChildComponents.Remove(oldComponent);
            await _dbContext.SaveChangesAsync();
        }

        // Validate component name
        var name = component.FullName;

        if (name is null)
            return new(false, new List<string>() { $"No qualified type name was able to be found for the provided {nameof(component)}." });

        // Add the new component type.
        PageComponentSettingsBase newComponent;
        if(component.GetCustomAttributes<LayoutComponentAttribute>()
            .FirstOrDefault() is not null)
        {
            // If the value is a layout componenet,
            // add a layout component settings object.
            newComponent = new LayoutComponentSettings();
        }
        else if (component.GetCustomAttributes<EditableComponentAttribute>()
            .FirstOrDefault() is not null)
        {
            // If the value is an editable component,
            // add an editable component settings object.
            newComponent = new EditableComponentSettings();
        }
        else if (component.GetCustomAttributes<DisplayComponentAttribute>()
            .FirstOrDefault() is not null)
        {
            // If the value is a display component,
            // add a display component settings object.
            newComponent = new DisplayComponentSettings();
        }
        else if (component.GetCustomAttributes<RosterComponentAttribute>()
            .FirstOrDefault() is not null)
        {
            // If the value is a roster component,
            // add a roster component settings object.
            newComponent = new RosterComponentSettings();
        }
        else if (component.GetCustomAttributes<ButtonComponentAttribute>()
            .FirstOrDefault() is not null)
        {
            // If the value is a button component,
            // add a button component settings object.
            newComponent = new ButtonComponentSettings();
        }
        else
        {
            // Otherwise, return the error.
            return new(false, new List<string>() { "No valid component type was provided." });
        }

        // Set the values for this component.
        newComponent.QualifiedTypeName = name;
        newComponent.Order = position;

        // Add the new child component.
        layoutData.ChildComponents.Add(newComponent);

        // Save changes and return true.
        await _dbContext.SaveChangesAsync();
        return new(true, null);
    }

    public async Task<ActionResult> DeleteLayoutComponentAsync(Guid layout)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the layout.
        var layoutData = await _dbContext.LayoutComponentSettings
            .Where(x => x.Key == layout)
            .Include(x => x.ParentLayout)
            .Include(x => x.ParentPage)
            .Include(x => x.ChildComponents)
            .FirstOrDefaultAsync();

        if (layoutData is null)
            return new(false, new List<string>() { "No layout settings object was found for the provided ID." });

        // Load the child components into a queue ...
        Queue<PageComponentSettingsBase> settings = new();
        foreach (var c in layoutData.ChildComponents)
            settings.Enqueue(c);
        // ... then load all child components under the item to delete ...
        while (settings.TryDequeue(out var c))
        {
            if (c is LayoutComponentSettings ls)
            {
                var cInstance = _dbContext.Entry(ls);
                await cInstance.Collection(x => x.ChildComponents).LoadAsync();
                foreach (var nextC in ls.ChildComponents)
                    settings.Enqueue(nextC);
            }
        }

        // ... then attempt to remove it.
        _dbContext.Remove(layoutData);
        try
        {
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string>() { "The layout component was unable to be deleted.", ex.Message });
        }
    }
    #endregion

    #region Editable Component Actions
    public async Task<ActionResult> DeleteEditableComponentAsync(Guid comp)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the layout.
        var compData = await _dbContext.EditableComponentSettings
            .Where(x => x.Key == comp)
            .Include(x => x.ParentLayout)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No editable component was found for the provided ID." });

        // ... then attempt to remove it.
        _dbContext.Remove(compData);
        try
        {
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string>() { "The editable component was unable to be deleted.", ex.Message });
        }
    }

    public async Task<ActionResult> UpdateEditableComponentAsync(Guid comp, 
        Action<ParameterComponentSettingsEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var compData = await _dbContext.EditableComponentSettings
            .Where(x => x.Key == comp)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No editable component was found for the provided ID." });

        ParameterComponentSettingsEditModel model = new();
        action.Invoke(model);

        ApplyParameterComponentEdits(compData, model);

        await _dbContext.SaveChangesAsync();
        return new(true, null);
    }
    #endregion

    #region Display Component Actions
    public async Task<ActionResult> DeleteDisplayComponentAsync(Guid comp)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the layout.
        var compData = await _dbContext.DisplayComponentSettings
            .Where(x => x.Key == comp)
            .Include(x => x.ParentLayout)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No display component was found for the provided ID." });

        // ... then attempt to remove it.
        _dbContext.Remove(compData);
        try
        {
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string>() { "The display component was unable to be deleted.", ex.Message });
        }
    }

    public async Task<ActionResult> UpdateDisplayComponentAsync(Guid comp, 
        Action<ParameterComponentSettingsEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var compData = await _dbContext.DisplayComponentSettings
            .Where(x => x.Key == comp)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No display component was found for the provided ID." });

        ParameterComponentSettingsEditModel model = new();
        action.Invoke(model);

        ApplyParameterComponentEdits(compData, model);

        await _dbContext.SaveChangesAsync();
        return new(true, null);
    }
    #endregion

    #region Roster Component Actions
    public async Task<ActionResult> DeleteRosterComponentAsync(Guid comp)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the layout.
        var compData = await _dbContext.RosterComponentSettings
            .Where(x => x.Key == comp)
            .Include(x => x.ParentLayout)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No roster component was found for the provided ID." });

        // ... then attempt to remove it.
        _dbContext.Remove(compData);
        try
        {
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string>() { "The roster component was unable to be deleted.", ex.Message });
        }
    }

    public async Task<ActionResult> UpdateRosterComponentAsync(Guid comp, Action<RosterComponentSettingsEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var compData = await _dbContext.RosterComponentSettings
            .Where(x => x.Key == comp)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No roster component was found for the provided ID." });

        RosterComponentSettingsEditModel model = new();
        action.Invoke(model);

        if(model.Scoped is not null)
            compData.Scoped = model.Scoped.Value;

        if(model.AllowUserListing is not null)
            compData.AllowUserLisiting = model.AllowUserListing.Value;

        if (model.UserListDisplayedProperties is not null)
        {
            // Remove any current that is missing from the new set ...
            foreach(var exisitng in compData.UserListDisplayedProperties)
            {
                if(!model.UserListDisplayedProperties.Any(x => x.Key == exisitng.Key))
                {
                    _dbContext.Remove(exisitng);
                }
            }
            // .. then add any completely new values ...
            var dataSet = model.UserListDisplayedProperties.Where(x => x.Key == Guid.Empty || !compData.UserListDisplayedProperties.Any(y => y.Key == x.Key));
            compData.UserListDisplayedProperties.AddRange(dataSet);
        }

        if (model.DefaultDisplayedProperties is not null)
        {
            // Remove any current that is missing from the new set ...
            foreach (var exisitng in compData.DefaultDisplayedProperties)
            {
                if (!model.DefaultDisplayedProperties.Any(x => x.Key == exisitng.Key))
                {
                    _dbContext.Remove(exisitng);
                }
            }
            // .. then add any completely new values ...
            var dataSet = model.DefaultDisplayedProperties.Where(x => x.Key == Guid.Empty || !compData.DefaultDisplayedProperties.Any(y => y.Key == x.Key));
            compData.DefaultDisplayedProperties.AddRange(dataSet);
        }

        if(model.LevelFromTop is not null)
            compData.LevelFromTop = model.LevelFromTop.Value;

        if(model.Depth is not null)
            compData.Depth = model.Depth.Value;

        if(model.AvalibleRosters is not null)
        {
            compData.AvalibleRosters = new();

            var keys = new List<Guid>();
            foreach (var i in model.AvalibleRosters)
                keys.Add(i.Key);

            await _dbContext.RosterDisplaySettings
                .Where(x => keys.Contains(x.Key))
                .ForEachAsync(x => compData.AvalibleRosters.Add(x));
        }

        try
        {
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string>() { "The roster component changes were unable to be saved.", ex.Message });
        }
    }
    #endregion

    #region Button Component Actions
    public async Task<ActionResult> DeleteButtonComponentAsync(Guid comp)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get the button.
        var compData = await _dbContext.ButtonComponentSettings
            .Where(x => x.Key == comp)
            .Include(x => x.ParentLayout)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No button component was found for the provided ID." });

        // ... then attempt to remove it.
        _dbContext.Remove(compData);
        try
        {
            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string>() { "The button component was unable to be deleted.", ex.Message });
        }
    }

    public async Task<ActionResult> UpdateButtonComponentAsync(Guid comp, Action<ButtonComponentSettingsEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var compData = await _dbContext.ButtonComponentSettings
            .Where(x => x.Key == comp)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No button component was found for the provided ID." });

        ButtonComponentSettingsEditModel model = new();
        action.Invoke(model);

        if(model.InvokeSave is not null)
            compData.InvokeSave = model.InvokeSave.Value;

        if(model.ResetForm is not null)
            compData.ResetForm = model.ResetForm.Value;

        if(model.Style is not null)
            compData.Style = model.Style.Value;

        if (model.DisplayName is not null)
            compData.DisplayName = model.DisplayName;

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }
    #endregion

    private static void ApplyParameterComponentEdits(ParameterComponentSettingsBase compData, 
        ParameterComponentSettingsEditModel model)
    {
        if (model.PropertyToEdit is not null)
            compData.PropertyToEdit = model.PropertyToEdit;

        if (model.StaticProperty is not null)
            compData.StaticProperty = model.StaticProperty.Value;

        if (model.Label.HasValue)
            compData.Label = model.Label.Value;

        if(model.UserScope.HasValue)
            compData.UserScopeId = model.UserScope.Value;
    }
}
