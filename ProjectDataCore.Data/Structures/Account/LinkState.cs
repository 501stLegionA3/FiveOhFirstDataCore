using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Account;
public class LinkState
{
    public AccountSettings LinkSettings { get; set; }

    public string Token { get; init; }
    public Guid UserId { get; set; }
    public ulong? DiscordId { get; init; }
    public string? DiscordEmail { get; set; }
    public string? SteamLink { get; init; }
    public bool LinkReady { get; init; }
    public Timer ExparationTimer { get; init; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }

    public LinkState(string token, Guid userId, Timer timer, string u, string p, bool r, 
        AccountSettings settings, ulong? discordId, string? steamLink, string? discordEmail = null)
    {
        Token = token;
        UserId = userId;
        ExparationTimer = timer;
        LinkReady = false;
        Username = u;
        Password = p;
        RememberMe = r;
        DiscordEmail = discordEmail;

        LinkSettings = settings;
        DiscordId = discordId;
        SteamLink = steamLink;

        if((!settings.RequireDiscordLink || discordId is not null)
            && (!settings.RequireSteamLink || steamLink is not null))
        {
            LinkReady = true;
        }
    }

    internal string GetNextRedirect(string token)
    {
        if (LinkSettings.RequireDiscordLink && DiscordId is null)
            return "/api/link/discord";
        else if (LinkSettings.RequireSteamLink && SteamLink is null)
            return "/api/link/steam";
        else return $"/api/link/token/{token}";
    }
}
