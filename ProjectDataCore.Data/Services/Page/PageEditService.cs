using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Services.Routing;
using ProjectDataCore.Data.Structures.Model.Page;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Attributes;
using ProjectDataCore.Data.Structures.Page.Components.Layout;
using ProjectDataCore.Data.Structures.Page.Components.Parameters;
using ProjectDataCore.Data.Structures.Policy;

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
    private readonly INavModuleService _navModuleService;

    public PageEditService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IRoutingService routingService, IScopedUserService scopedUserService, INavModuleService navModuleService)
        => (_dbContextFactory, _routingService, _scopedUserService, _navModuleService) = (dbContextFactory, routingService, scopedUserService, navModuleService);

    #region Page Actions
    public async Task<ActionResult> CreateNewPageAsync(string name, string route)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        // create a new page to add ...
        var obj = new CustomPageSettings()
        {
            Name = name,
            Route = route,
            Layout = new()
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

        var nav = await _dbContext.NavModules.Where(e => e.PageId == page).ToListAsync();
        foreach (var m in nav)
        {
            m.PageId = null;
        }

        _dbContext.Remove(obj);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<List<CustomPageSettings>> GetAllPagesAsync()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await _dbContext.CustomPageSettings.Include(e=>e.Layout).ToListAsync();
    }
    // TODO REWORK THIS
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
            Queue<LayoutNode> level = new();
            level.Enqueue(obj.Layout);

            // ... and while we have level data ...
            while (level.TryDequeue(out var levelItem))
            {
                // ... attach the level item ...
                var layoutObj = _dbContext.Attach(levelItem);
                // ... and load its direct children ...
                var children = layoutObj.Collection(x => x.Nodes).Query()
                    .Include(x => x.ParentNode)
                    .AsAsyncEnumerable();

                // ... then for each child ...
                await foreach (var child in children)
                {
                    // ... if the child is a layout component, enqueue it.
                    if (child is LayoutNode childLayout)
                        level.Enqueue(childLayout);
                }
            }
        }

        // ... and return the fully loaded settings object.
        return new(true, null, obj);
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
        {
            pageData.Route = update.Route;
            var nav = await _dbContext.NavModules.Where(e => e.PageId == page).ToListAsync();
            foreach (var m in nav)
            {
                m.Href = pageData.Route;
            }
        }

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

    public async Task<ActionResult> UpdateTextDisplayContentsAsync(Guid comp, string rawContents)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var compData = await _dbContext.TextDisplayComponentSettings
            .Where(x => x.Key == comp)
            .FirstOrDefaultAsync();

        if (compData is null)
            return new(false, new List<string>() { "No display component was found for the provided ID." });

        if (rawContents is not null)
            compData.RawContents = rawContents;
        else compData.RawContents = "";

        await _dbContext.SaveChangesAsync();
        return new(true, null);
    }

    private static void ApplyParameterComponentEdits(ParameterComponentSettingsBase compData, 
        ParameterComponentSettingsEditModel model)
    {
        if (model.PropertyToEdit is not null)
            compData.PropertyName = model.PropertyToEdit;

        if (model.StaticProperty is not null)
            compData.StaticProperty = model.StaticProperty.Value;

        if(model.UserScope.HasValue)
            compData.UserScopeId = model.UserScope.Value;
    }
}
