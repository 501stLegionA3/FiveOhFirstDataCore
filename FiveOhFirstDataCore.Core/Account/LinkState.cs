namespace FiveOhFirstDataCore.Data.Account
{
    public class LinkState
    {
        public string Token { get; init; }
        public int TrooperId { get; set; }
        public string? DiscordId { get; init; }
        public string? DiscordEmail { get; set; }
        public string? SteamId { get; init; }
        public bool LinkReady { get; init; }
        public Timer ExparationTimer { get; init; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public LinkState(string token, int trooperId, Timer timer, string u, string p, bool r)
        {
            Token = token;
            TrooperId = trooperId;
            ExparationTimer = timer;
            LinkReady = false;
            Username = u;
            Password = p;
            RememberMe = r;
        }

        public LinkState(string token, int trooperId, string discordId, string discordEmail, Timer timer, string u, string p, bool r)
        {
            Token = token;
            TrooperId = trooperId;
            DiscordId = discordId;
            DiscordEmail = discordEmail;
            ExparationTimer = timer;
            LinkReady = false;
            Username = u;
            Password = p;
            RememberMe = r;
        }

        public LinkState(string token, int trooperId, string discordId, string? discordEmail, string steamId, Timer timer, string u, string p, bool r)
        {
            Token = token;
            TrooperId = trooperId;
            DiscordId = discordId;
            DiscordEmail = discordEmail;
            SteamId = steamId;
            ExparationTimer = timer;
            LinkReady = true;
            Username = u;
            Password = p;
            RememberMe = r;
        }
    }
}
