using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Account;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FiveOhFirstDataCore.Components.Promotions
{
    public partial class RazorPromotionBoard
    {
        [Parameter]
        public RazorSquadronData Razor { get; set; }

        [Parameter]
        public int Slot { get; set; } = -1;
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
            if (Razor.SubCommander is not null)
                Troopers.Add(Razor.SubCommander);
            foreach (var flight in Razor.Flights)
            {
                if (flight.Commander is not null)
                    Troopers.Add(flight.Commander);
                foreach (var section in flight.Sections)
                {
                    if (section.Alpha is not null)
                        Troopers.Add(section.Alpha);
                    if (section.Bravo is not null)
                        Troopers.Add(section.Bravo);
                    if (section.Charlie is not null)
                        Troopers.Add(section.Charlie);
                    if (section.Delta is not null)
                        Troopers.Add(section.Delta);
                }
            }

            if (CanPromote)
                CanPromoteValues = Troopers.ToHashSet(x => x.Id);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            var user = (await AuthStateTask).User;
            bool manager = (await _auth.AuthorizeAsync(user, "RequireManager")).Succeeded || user.HasClaim("Promotion.Razor", "Command");
            CanPromote = manager || (Razor?.Commander?.Id ?? 0) == CurrentUser?.Id || (Razor?.SubCommander?.Id ?? 0) == CurrentUser?.Id;
            BuildTrooperList();
        }
    }
}