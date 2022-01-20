using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Data.Services.Nav;

public class NavModuleService : INavModuleService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public event EventHandler<NavModule> OnDblClick;
    public event EventHandler<NavModule> OnLeftClick;

    public NavModuleService(IDbContextFactory<ApplicationDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;
    
    public async Task<List<NavModule>> GetAllModules()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await _dbContext.NavModules.ToListAsync();
    }

    public async Task<List<NavModule>> GetAllModulesWithChildren()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        var modules =  await _dbContext.NavModules.Where(e => e.ParentId == null).ToListAsync();
        foreach(var module in modules)
            await LoadNavModule(module);
        return modules;
    }

    protected async Task LoadNavModule(NavModule item)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        var obj = _dbContext.Attach(item);
            await obj.Collection(e => e.SubModules)
                .LoadAsync();

            Queue<NavModule> navModules = new();
            foreach (var t in item.SubModules)
                navModules.Enqueue(t);

            while (navModules.TryDequeue(out var module))
            {
                obj = _dbContext.Entry(module);
                await obj.Collection(e => e.SubModules)
                    .LoadAsync();

                foreach (var t in module.SubModules)
                    navModules.Enqueue(t);
            }
        return;
    }

    public async Task<ActionResult> CreateNavModuleAsync(string displayName, string href, bool hasMainPage, Guid? parent = null)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        try
        {
            _dbContext.Add(new NavModule(displayName, href, hasMainPage) { ParentId = parent });
            await _dbContext.SaveChangesAsync();
            return new(true);
        }
        catch (Exception ex)
        {
            return new(false, new() { ex.Message });
        }

    }

    public async Task<ActionResult> CreateNavModuleAsync(NavModule module)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        try
        {
            _dbContext.Add(module);
            await _dbContext.SaveChangesAsync();
            return new(true);
        }
        catch (Exception ex)
        {
            return new(false, new() { ex.Message });
        }
    }

    public async Task<ActionResult> DeleteNavModule(Guid key)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        NavModule module = await _dbContext.NavModules.FindAsync(key);
        await LoadNavModule(module);
        if (module.SubModules.Any())
            foreach (var subModule in module.SubModules)
            {
                _dbContext.Remove(subModule);
            }
        _dbContext.Remove(module);
        _dbContext.SaveChanges();
        return new(true);
    }

    public async Task<ActionResult> UpdateNavModuleAsync(NavModule navModule)
    {
        if (navModule == null)
        {
            return new(false, new() {"paramater is null"});
        }
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        NavModule module = await _dbContext.FindAsync<NavModule>(navModule.Key);
        if (module is null)
        {
            return new(false, new() { "Module not found" });
        }
        if (module.HasMainPage != navModule.HasMainPage)
            module.HasMainPage = navModule.HasMainPage;
        if (module.Href != navModule.Href)
            module.Href = navModule.Href;
        if (module.ParentId!= navModule.ParentId)
            module.ParentId = navModule.ParentId;
        if (module.DisplayName != navModule.DisplayName)
            module.DisplayName = navModule.DisplayName;
        await _dbContext.SaveChangesAsync();
        return new(true);
    }

    public void OnLeftClickTrigger(object? sender, NavModule module)
    {
        try
        {
            OnLeftClick.Invoke(sender, module);
        }
        catch (Exception)
        {

        }
    }

    public void OnDblClickTrigger(object? sender, NavModule module)
    {
        try
        {
            OnDblClick.Invoke(sender, module);
        }
        catch (Exception)
        {

        }
    }
}