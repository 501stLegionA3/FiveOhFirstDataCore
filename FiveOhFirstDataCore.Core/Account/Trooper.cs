using FiveOhFirstDataCore.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Account
{
    public class Trooper : IdentityUser<int>
    {
        public string NickName { get; set; } = "";

        public TrooperRank Rank { get; set; }
        public RTORank? RTORank { get; set; }
        public MedicRank? MedicRank { get; set; }
        public PilotRank? PilotRank { get; set; }

        public Slot Slot { get; set; }
        public Role Role { get; set; }
        public Team? Team { get; set; }
        public Flight? Flight { get; set; }

        public DateTime LastPromotion { get; set; }
        public DateTime StartOfService { get; set; }

        public string? InitalTraining { get; set; }
        public string? UTC { get; set; }

        public string Notes { get; set; } = "";

        public string? DiscordId { get; set; }
        public string? SteamLink { get; set; }
        public string? AccessCode { get; set; }
    }
}
