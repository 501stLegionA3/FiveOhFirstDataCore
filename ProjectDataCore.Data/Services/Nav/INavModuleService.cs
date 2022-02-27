using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Data.Services.Nav;

public interface INavModuleService
{
    public Task<ActionResult<Guid>> CreateNavModuleAsync(string displayName, string href, bool hasMainPage, Guid? parent = null);
    public Task<ActionResult> CreateNavModuleAsync(NavModule module);
    public Task<List<NavModule>> GetAllModules();
    public Task<List<NavModule>> GetAllModulesWithChildren();
    public Task<ActionResult> UpdateNavModuleAsync(NavModule navModule);
    public Task<ActionResult> DeleteNavModule(Guid key);
}