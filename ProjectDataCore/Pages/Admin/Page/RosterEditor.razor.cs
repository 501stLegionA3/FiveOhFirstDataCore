using ProjectDataCore.Data.Structures.Roster;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Page;
public partial class RosterEditor : ComponentBase
{
#pragma warning disable CS8618 // Injections are never null.
	[Inject]
	public IModularRosterService ModularRosterService { get; set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public string NewRosterName { get; set; } = "";
    public string TopLevelSectionName { get; set; } = "";

    public string? Error { get; set; }

    public List<RosterDisplaySettings> AllRosterDisplays { get; set; } = new();
    public List<RosterTree> AllOrphanedRosterTrees { get; set; } = new();
    public List<RosterTree> AllTopLevelRosterTrees { get; set; } = new();

    public RosterTree? EditTree { get; set; }
    public Func<bool, Task>? FullReloadListener { get; set; }

    public ConcurrentDictionary<Guid, bool> OpenRosterEdits { get; set; } = new();

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

        FullReloadListener = new(CallFullReload);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if(firstRender)
		{
			await ReloadRosterDisplaysAsync();
		}
	}

    protected async Task ReloadRosterDisplaysAsync()
    {
        // Get all avalible rosters to display ...
        var displayRes = await ModularRosterService.GetAvalibleRosterDisplaysAsync();

        if (displayRes.GetResult(out var displays, out var err))
        {
            // ... and add them as options.
            AllRosterDisplays = displays;
        }
        else
        {
            Error = err[0];
            return;
        }

        var oprhanRes = await ModularRosterService.GetOrphanedRosterTreesAsync();

        if (oprhanRes.GetResult(out var trees, out err))
        {
            // ... and add them as options.
            AllOrphanedRosterTrees = trees;
        }
        else
        {
            Error = err[0];
            return;
        }

        var topRes = await ModularRosterService.GetTopLevelRosterTreesAsync();

        if (topRes.GetResult(out trees, out err))
        {
            // ... and add them as options.
            AllTopLevelRosterTrees = trees;
        }
        else
        {
            Error = err[0];
            return;
        }

        StateHasChanged();
    }

    protected async Task AddNewRosterDisplay()
	{
        Error = null;

        if(!string.IsNullOrWhiteSpace(NewRosterName)
            && !string.IsNullOrWhiteSpace(TopLevelSectionName))
		{
            var keyRes = await ModularRosterService.AddRosterTreeAsync(TopLevelSectionName);

            if(keyRes.GetResult(out var key, out var err))
			{
                var res = await ModularRosterService.AddRosterDisplaySettingsAsync(NewRosterName, key);

                _ = res.GetResult(out err);
			}

            if(err is not null)
			{
                Error = err[0];
			}

            await ReloadRosterDisplaysAsync();
		}
        else
		{
            Error = "Both roster display name and roster tree name must be set.";
		}
	}

    protected async Task AddNewRosterTree()
	{
        Error = null;

        if (!string.IsNullOrWhiteSpace(TopLevelSectionName))
        {
            var res = await ModularRosterService.AddRosterTreeAsync(TopLevelSectionName);

            if (!res.GetResult(out var err))
                Error = err[0];
            else
                await ReloadRosterDisplaysAsync();
        }
        else
		{
            Error = "Roster Tree name must have a value.";
		}
	}

    protected async Task EditRosterFromDisplayAsync(RosterDisplaySettings display)
	{
        var res = await ModularRosterService.GetRosterTreeForSettingsAsync(display.Key);

        if (res.GetResult(out var tree, out var err))
            EditTree = tree;
        else
            Error = err[0];

        _ = Task.Run(async () => await LoadRosterAsync());
	}

    protected async Task DeleteRosterDisplayAsync(RosterDisplaySettings display)
	{
        var res = await ModularRosterService.RemoveRosterDisplaySettingsAsync(display.Key);

        if (!res.GetResult(out var err))
            Error = err[0];

        await ReloadRosterDisplaysAsync();
	}

    protected void EditRosterTree(RosterTree tree)
	{
        EditTree = tree;

        _ = Task.Run(async () => await LoadRosterAsync());
	}

    protected async Task DeleteRosterTreeAsync(RosterTree tree)
	{
        var res = await ModularRosterService.RemoveRosterTreeAsync(tree.Key);

        if (!res.GetResult(out var err))
            Error = err[0];

        await ReloadRosterDisplaysAsync();
	}

    protected async Task LoadRosterAsync(bool singleRefresh = false)
	{
        if (EditTree is not null)
        {
            var editRes = await ModularRosterService.GetRosterTreeByIdAsync(EditTree.Key);

            if (editRes.GetResult(out var editTree, out _))
            {
                EditTree = editTree;

                var loader = ModularRosterService.LoadFullRosterTreeAsync(EditTree);

                if (!singleRefresh)
                {
                    await foreach (var _ in loader)
                        await InvokeAsync(StateHasChanged);
                }
                else
                {
                    await foreach (var _ in loader) { }
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
	}

    protected async Task CallFullReload(bool singleRefresh)
	{
        await ReloadRosterDisplaysAsync();

        _ = Task.Run(async () => await LoadRosterAsync(singleRefresh));
    }
}
