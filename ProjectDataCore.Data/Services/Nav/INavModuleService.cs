using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Data.Services.Nav;

public interface INavModuleService
{
    public event EventHandler<NavModule> OnDblClick;
    public event EventHandler<NavModule> OnLeftClick;
    public void OnLeftClickTrigger(object? sender, NavModule module);
    public void OnDblClickTrigger(object? sender, NavModule module);
    public Task<ActionResult> CreateNavModuleAsync(string displayName, string href, bool hasMainPage, Guid? parent = null);
    public Task<ActionResult> CreateNavModuleAsync(NavModule module);
    public Task<List<NavModule>> GetAllModules();
    public Task<List<NavModule>> GetAllModulesWithChildren();
    Task<ActionResult> UpdateNavModuleAsync(NavModule navModule);
    Task<ActionResult> DeleteNavModule(Guid key);
}