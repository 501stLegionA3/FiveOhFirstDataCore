using FiveOhFirstDataCore.Core.Structures.Policy;
using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Auth;
public partial class DynamicPolicyEditor
{
    [Inject]
    public IWebsiteSettingsService WebsiteSettingsService { get; set; }
    [Inject]
    public IAlertService AlertService {  get; set; }
    [Parameter]
    public DynamicPolicy? ToEdit { get; set; } = null;
    [Parameter]
    public bool CloseOnSave { get; set; } = false;
    [Parameter]
    public bool IndependentDisplay { get; set; } = false;
    private bool ExsistingPolicy { get; set; } = false;

    private PolicyClaimData AddClaim { get; set; } = new();
    private WebsiteRoles WebsiteRole { get; set; } = WebsiteRoles.Admin;

    private Dictionary<CShop, CShopClaim> CShopClaims { get; set; } = new();

    private CShop ClaimGroup { get; set; } = CShop.None;
    private string CShopGroup { get; set; } = "";
    private string CShopPosition { get; set; } = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            if (ToEdit is not null)
            {
                ToEdit = await WebsiteSettingsService.GetDynamicPolicyAsync(ToEdit.PolicyName);
            }

            if (ToEdit is not null)
                ExsistingPolicy = true;
            else
                ToEdit = new();

            CShopClaims = await WebsiteSettingsService.GetFullClaimsTreeAsync();

            StateHasChanged();
        }
    }

    private async Task OnSave()
    {
        if (!string.IsNullOrWhiteSpace(ToEdit!.PolicyName))
        {
            await WebsiteSettingsService.UpdateOrCreatePolicyAsync(ToEdit);
            AlertService.PostAlert(this, $"A new policy by the name of {ToEdit.PolicyName} has been created or updated.");
            ExsistingPolicy = false;
            ToEdit = new();
            if (CloseOnSave)
                AlertService.PostModal(this, null);
        }
        else
        {
            AlertService.PostAlert(this, new List<string>() { "A policy must have a name!" });
        }
    }

    private void OnClose()
    {
        AlertService.PostModal(this, null);
    }

    private void AddClaimPair()
    {
        ToEdit!.RequiredClaims.Add(AddClaim);
        AddClaim = new();
        StateHasChanged();
    }

    private void AddCShopGroup()
    {
        foreach(var i in CShopClaims[ClaimGroup].ClaimData[CShopGroup])
        {
            CShopPosition = i;
            AddCShopPosition();
        }
        CShopGroup = "";
        ClaimGroup = CShop.None;
        StateHasChanged();
    }

    private void AddCShopPosition()
    {
        ToEdit!.RequiredClaims.Add(new()
        {
            Claim = CShopGroup,
            Value = CShopPosition
        });
        CShopPosition = "";
        StateHasChanged();
    }

    private void RemoveClaimPair(PolicyClaimData pair)
    {
        ToEdit!.RequiredClaims.Remove(pair);
        StateHasChanged();
    }

    private void AddRole()
    {
        ToEdit!.RequiredRoles.Add(WebsiteRole.AsFull());
        StateHasChanged();
    }

    private void RemoveRole(string role)
    {
        ToEdit!.RequiredRoles.Remove(role);
        StateHasChanged();
    }
}
