using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Components.Promotions
{
    public partial class WardenPromotionBoard
    {
        [Parameter]
        public WardenData Warden { get; set; }
        [Parameter]
        public RazorSquadronData Razor { get; set; }

        [CascadingParameter]
        public Trooper CurrentUser { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        public List<Trooper>? Troopers { get; set; } = null;
        private bool CanPromote { get; set; } = false;
        private HashSet<int> CanPromoteValues { get; set; } = new();
        private void BuildTrooperList()
        {
            Troopers = new();
            if (Warden.Chief is not null)
                Troopers.Add(Warden.Chief);
            foreach (var warden in Warden.Wardens)
            {
                if (warden.SectionLead is not null)
                    Troopers.Add(warden.SectionLead);
                foreach (var t in warden.TeamOne)
                    if (t is not null)
                        Troopers.Add(t);
                foreach (var t in warden.TeamTwo)
                    if (t is not null)
                        Troopers.Add(t);
            }

            if (CanPromote)
                CanPromoteValues = Troopers.ToHashSet(x => x.Id);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            var user = (await AuthStateTask).User;
            bool manager = (await _auth.AuthorizeAsync(user, "RequireManager")).Succeeded || user.HasClaim("Promotion.Razor", "Warden");
            // Set can promote value inlcuding Razor command.
            CanPromote = manager || (Warden?.Master?.Id ?? 0) == CurrentUser?.Id || (Warden?.Chief?.Id ?? 0) == CurrentUser?.Id
                || (Razor?.Commander?.Id ?? 0) == CurrentUser?.Id || (Razor?.SubCommander?.Id ?? 0) == CurrentUser?.Id;
            BuildTrooperList();
        }
    }
}