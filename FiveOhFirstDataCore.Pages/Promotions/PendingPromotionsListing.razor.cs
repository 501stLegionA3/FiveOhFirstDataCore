using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Promotions
{
    public partial class PendingPromotionsListing
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        [CascadingParameter(Name = "CanPromote")]
        public bool AllowedPromoter { get; set; }

        [CascadingParameter]
        public HashSet<int> CanPromote { get; set; } = new();

        [Parameter]
        public List<Trooper>? ActiveTroopers { get; set; }

        [CascadingParameter]
        public PromotionBoardLevel BoardLevel { get; set; } = PromotionBoardLevel.Squad;

        [Parameter]
        public bool Mynock { get; set; } = false;

        [CascadingParameter(Name = "Airborne")]
        public bool Airborne { get; set; } = false;

        public int Counter { get; set; } = 0;

        private int ConfirmApprove { get; set; } = -1;
        private int ConfirmDeny { get; set; } = -1;

        private HashSet<Guid> ForcedPromotions { get; set; } = new();

        public bool EditingReason { get; set; } = false;
        public string NewReason { get; set; } = string.Empty;

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }
        private bool Manager { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (ActiveTroopers is not null)
            {
                foreach (var t in ActiveTroopers)
                {
                    var set = t.PendingPromotions.Where(x => x.Forced);

                    if (set.Count() > 0)
                    {
                        var eligible = await _settings.GetEligiblePromotionsAsync(t, false, false);
                        foreach (var i in set)
                        {
                            if (!eligible.Any(x => x.PromoteFrom == i.PromoteFrom
                                 && x.PromoteTo == i.PromoteTo))
                            {
                                ForcedPromotions.Add(i.Id);
                            }
                            else
                            {
                                await _settings.RemoveForcedTagAsync(i);
                            }
                        }
                    }
                }
            }

            var user = (await AuthStateTask).User;
            Manager = user.IsInRole("Manager") || user.IsInRole("Admin");
        }

        protected async Task ApprovePromotion(Promotion promo, int c)
        {
            if (ConfirmApprove == c)
            {
                ConfirmApprove = -1;

                int levels = 1;
                if (BoardLevel == PromotionBoardLevel.Squad && Airborne)
                    levels = 2;
                var res = await _promote.ElevatePromotionAsync(promo, CurrentUser, levels);

                if (!res.GetResult(out var errors))
                {
                    _alert.PostAlert(this, errors);
                }
                else
                {
                    if (promo.NeededBoard == BoardLevel
                        || (promo.NeededBoard >= BoardLevel && promo.NeededBoard < PromotionBoardLevel.Razor))
                    {
                        _alert.PostAlert(this, $"Approved the promotion for {promo.PromotionFor?.NickName}. This promotion is complete.");
                    }
                    else
                    {
                        _alert.PostAlert(this, $"Approved the promotion for {promo.PromotionFor?.NickName}. This promotion is has been passed up.");
                    }
                }

                if (BoardLevel >= PromotionBoardLevel.Company
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Battalion");
                }
                if (BoardLevel >= PromotionBoardLevel.Platoon
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Company");
                    if (Mynock)
                    {
                        _advRefresh.CallDataReloadRequest("Promotion_Mynock_Command");
                    }
                }
                if (BoardLevel >= PromotionBoardLevel.Squad
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    if (Airborne)
                        _advRefresh.CallDataReloadRequest("Promotion_Company");
                    else
                        _advRefresh.CallDataReloadRequest("Promotion_Platoon");
                }

                if (BoardLevel == PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Razor");
                else if (BoardLevel == PromotionBoardLevel.Warden)
                    _advRefresh.CallDataReloadRequest("Promotion_Warden");
                else
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Squad");
                    if (Mynock)
                    {
                        _advRefresh.CallDataReloadRequest("Promotion_Mynock_Section");
                    }
                }

                StateHasChanged();
            }
        }

        protected async Task DenyPromotion(Promotion promo, int c)
        {
            if (ConfirmDeny == c)
            {
                ConfirmDeny = -1;

                var res = await _promote.CancelPromotionAsync(promo, CurrentUser);

                if (!res.GetResult(out var errors))
                {
                    _alert.PostAlert(this, errors);
                }
                else
                {
                    _alert.PostAlert(this, $"Denied the promotion for {promo.PromotionFor?.NickName}.");
                }

                if (BoardLevel >= PromotionBoardLevel.Battalion
                    && BoardLevel < PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Battalion");
                if (BoardLevel >= PromotionBoardLevel.Company
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Company");
                    if (Mynock)
                    {
                        _advRefresh.CallDataReloadRequest("Promotion_Mynock_Command");
                    }
                }
                if (BoardLevel >= PromotionBoardLevel.Platoon
                    && BoardLevel < PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Platoon");
                if (BoardLevel >= PromotionBoardLevel.Squad
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Squad");
                    if (Mynock)
                    {
                        _advRefresh.CallDataReloadRequest("Promotion_Mynock_Section");
                    }
                }
                if (BoardLevel == PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Razor");
                else if (BoardLevel == PromotionBoardLevel.Warden)
                    _advRefresh.CallDataReloadRequest("Promotion_Warden");

                if (BoardLevel >= PromotionBoardLevel.Battalion
                    && BoardLevel < PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Battalion");
                if (BoardLevel >= PromotionBoardLevel.Company
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Company");
                    if (Mynock)
                    {
                        _advRefresh.CallDataReloadRequest("Promotion_Mynock_Command");
                    }
                }
                if (BoardLevel >= PromotionBoardLevel.Platoon
                    && BoardLevel < PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Platoon");
                if (BoardLevel >= PromotionBoardLevel.Squad
                    && BoardLevel < PromotionBoardLevel.Razor)
                {
                    _advRefresh.CallDataReloadRequest("Promotion_Squad");
                    if (Mynock)
                    {
                        _advRefresh.CallDataReloadRequest("Promotion_Mynock_Section");
                    }
                }
                if (BoardLevel == PromotionBoardLevel.Razor)
                    _advRefresh.CallDataReloadRequest("Promotion_Razor");
                else if (BoardLevel == PromotionBoardLevel.Warden)
                    _advRefresh.CallDataReloadRequest("Promotion_Warden");

                StateHasChanged();
            }
        }

        protected async Task EditPromotion(Promotion promo, string reason)
        {
            var result = await _promote.UpdatePromotionAsync(promo, NewReason);

            if (result.GetResult(out var e)){ promo.Reason = reason; } else Console.WriteLine(e);
        }
    }
}
