using DSharpPlus.Entities;

using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Discord;
using FiveOhFirstDataCore.Data.Structures.Notice;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Settings;
public partial class RoleBindingSettingDisplay
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IWebsiteSettingsService Settings { get; set; }
    [Inject]
    public IAlertService Alert { get; set; }
    [Inject]
    public IDiscordService Discord { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public List<DiscordRoleDetails> DiscordRoleBindings { get; set; } = new();
    public List<CShopRoleBindingData> CShopRoleBindings { get; set; } = new();

    private string NewDiscordBindingRaw { get; set; } = "";

    private CShop? CShopCluster { get; set; } = null;
    private string? CShopDept { get; set; } = null;
    private string NewCShopBindingRaw { get; set; } = "";

    private Dictionary<CShop, CShopClaim> CShopClaims { get; set; } = new();

    private IReadOnlyList<DiscordRole> DiscordRoles { get; set; } = Array.Empty<DiscordRole>();

    private IReadOnlyList<DiscordChannel> DiscordChannels { get; set; } = Array.Empty<DiscordChannel>();
    private ulong[] Channel { get; set; } = new ulong[1];
    private string RawMessage { get; set; } = "";
    private DiscordPostActionConfiguration ActionConfig { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await OnRevertAll();
        }
    }

    public async Task OnRevertAll()
    {
        DiscordRoleBindings = await Settings.GetAllDiscordBindingsAsync();
        CShopRoleBindings = await Settings.GetAllCShopRoleBindingDataAsync();
        CShopClaims = await Settings.GetFullClaimsTreeAsync();
        DiscordRoles = await Discord.GetAllHomeGuildRolesAsync();
        DiscordChannels = await Discord.GetAllHomeGuildChannelsAsync();
        ActionConfig = await Settings.GetDiscordPostActionConfigurationAsync(DiscordAction.TagUpdate) ??  new() { Action = DiscordAction.TagUpdate };
        RawMessage = ActionConfig.RawMessage;
        Channel[0] = DiscordChannels.FirstOrDefault(x => x.Id == ActionConfig.DiscordChannel)?.Id ?? 0;
        StateHasChanged();
    }

    private void OnAddNewDiscordBinding()
    {
        if (!string.IsNullOrWhiteSpace(NewDiscordBindingRaw))
        {
            DiscordRoleDetails newDiscordBinding = new()
            {
                Key = NewDiscordBindingRaw
            };

            DiscordRoleBindings.Add(newDiscordBinding);
        }

        NewDiscordBindingRaw = "n/a";
    }

    private async void OnAddNewCShopBinding()
    {
        if (!string.IsNullOrWhiteSpace(NewCShopBindingRaw))
        {
            CShopRoleBindingData newCShopBinding = new()
            {
                Id = NewCShopBindingRaw
            };

            CShopRoleBindings.Add(newCShopBinding);
        }

        NewDiscordBindingRaw = "n/a";
    }

    private async Task SaveBinding(DiscordRoleDetails bind)
    {
        var res = await Settings.AddOrUpdateDiscordBindingsAsync(bind);
        if(!res.GetResult(out var err))
        {
            Alert.PostAlert(this, err);
        }

        await RevertBinding(bind);
    }

    private async Task RevertBinding(DiscordRoleDetails bind)
    {
        var index = DiscordRoleBindings.IndexOf(bind);

        var newbind = await Settings.GetDiscordRoleDetailsAsync(bind.Key);

        if (newbind is not null)
        {
            DiscordRoleBindings[index] = newbind;
        }
        else
        {
            DiscordRoleBindings.RemoveAt(index);
        }
    }

    private async Task SaveBinding(CShopRoleBindingData bind)
    {
        if (bind.Parent is not null)
        {
            CShopDept = bind.Parent.Id;
            if(bind.Parent.Parent is not null)
            {
                CShopCluster = bind.Parent.Parent.Key;
            }
        }

        if (CShopCluster is not null)
        {
            var clust = await Settings.ValidateCShopRoleBindClusterAsync(CShopCluster.Value);
            if (clust is not null && CShopDept is not null)
            {
                var dept = await Settings.ValidateCShopRoleBindDepartmentAsync(CShopDept, clust.Value);
                if (dept is not null)
                {
                    bind.ParentKey = dept.Value;

                    var res = await Settings.AddOrUpdateCShopRoleBindingAsync(bind);
                    if (!res.GetResult(out var err))
                    {
                        Alert.PostAlert(this, err);
                    }

                    await RevertBinding(bind);
                }
                else
                {
                    Alert.PostAlert(this, new AlertData()
                    {
                        Content = "Failed to parse a C-Shop Department.",
                        Level = AlertLevel.Danger
                    });
                }
            }
            else
            {
                Alert.PostAlert(this, new AlertData()
                {
                    Content = "Failed to parse a C-Shop Cluster.",
                    Level = AlertLevel.Danger
                });
            }
        }
    }

    private async Task RevertBinding(CShopRoleBindingData bind)
    {
        var index = CShopRoleBindings.IndexOf(bind);

        var newbind = await Settings.GetCShopRoleBindingAsync(bind.Key);

        if (newbind is not null)
        {
            CShopRoleBindings[index] = newbind;
        }
        else
        {
            CShopRoleBindings.RemoveAt(index);
        }
    }

    private async Task SavePostAction()
    {
        await Settings.UpdateDiscordPostActionConfigurationAsync(DiscordAction.TagUpdate, Channel[0], RawMessage);
        await RevertPostAction();
    }

    private async Task RevertPostAction()
    {
        DiscordChannels = await Discord.GetAllHomeGuildChannelsAsync();
        ActionConfig = await Settings.GetDiscordPostActionConfigurationAsync(DiscordAction.TagUpdate) ?? new() { Action = DiscordAction.TagUpdate };
        RawMessage = ActionConfig.RawMessage;
        Channel[0] = DiscordChannels.FirstOrDefault(x => x.Id == ActionConfig.DiscordChannel)?.Id ?? 0;
    }
}
