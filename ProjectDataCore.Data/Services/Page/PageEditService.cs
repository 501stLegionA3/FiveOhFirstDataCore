using ProjectDataCore.Data.Structures.Model.Page;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Page;

public class PageEditService : IPageEditService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public PageEditService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => (_dbContextFactory) = (dbContextFactory);

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
}
