using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Account
{
    public class LinkState
    {
		public string Token { get; init; }
		public int TrooperId { get; set; }
		public ulong? DiscordId { get; init; }
		public string? SteamId { get; init; }
		public bool LinkReady { get; init; }
		public Timer ExparationTimer { get; init; }

		public LinkState(string token, int trooperId, Timer timer)
		{
			Token = token;
			TrooperId = trooperId;
			ExparationTimer = timer;
			LinkReady = false;
		}

		public LinkState(string token, int trooperId, ulong discordId, Timer timer)
		{
			Token = token;
			TrooperId = trooperId;
			DiscordId = discordId;
			ExparationTimer = timer;
			LinkReady = false;
		}

		public LinkState(string token, int trooperId, ulong discordId, string steamId, Timer timer)
		{
			Token = token;
			TrooperId = trooperId;
			DiscordId = discordId;
			SteamId = steamId;
			ExparationTimer = timer;
			LinkReady = true;
		}
	}
}
