using Microsoft.Extensions.DependencyInjection;

using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Account;
using ProjectDataCore.Data.Structures.Model.Account;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Account;
public class AccountLinkService : IAccountLinkService
{
    public enum LinkStatus
    {
        Ready,
        Waiting,
        Invalid
    }

    public ConcurrentDictionary<string, LinkState> LinkingController { get; init; }
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public AccountLinkService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        LinkingController = new();
    }

    public async Task<LinkSettings> GetLinkSettingsAsync()
    {
        await using var _database = await _dbContextFactory.CreateDbContextAsync();

        var settings = await _database.LinkSettings
            .FirstOrDefaultAsync();

        if(settings is null)
        {
            settings = new();
            var val = await _database.AddAsync(settings);
            await _database.SaveChangesAsync();
            await val.ReloadAsync();
        }

        return settings;
    }

    public async Task UpdateLinkSettingsAsync(Action<LinkSettingsEditModel> action)
    {
        var settings = await GetLinkSettingsAsync();

        await using var _database = await _dbContextFactory.CreateDbContextAsync();

        _database.Attach(settings);

        LinkSettingsEditModel model = new();
        action.Invoke(model);

        if (model.RequireSteamLink is not null)
            settings.RequireSteamLink = model.RequireSteamLink.Value;

        if(model.RequireDiscordLink is not null)
            settings.RequireDiscordLink = model.RequireDiscordLink.Value;

        await _database.SaveChangesAsync();
    }

    public async Task<string> StartAsync(Guid userId, string username, string password, bool rememberMe)
    {
        await using var _database = await _dbContextFactory.CreateDbContextAsync();
        var user = await _database.FindAsync<DataCoreUser>(userId);
        if (user is null)
            throw new Exception($"The trooper is null.");

        var settings = await GetLinkSettingsAsync();

        var token = Guid.NewGuid().ToString();

        LinkingController[token] = new(token, userId, new Timer((x) =>
        {
            _ = LinkingController.TryRemove(token, out _);
        },
        null, TimeSpan.FromMinutes(1.5), Timeout.InfiniteTimeSpan), username, password, rememberMe, 
            settings, user.DiscordId, user.SteamLink, user.Email);

        return token;
    }

    public async Task<string> BindDiscordAsync(string token, ulong accountId, string email)
    {
        await using var _database = await _dbContextFactory.CreateDbContextAsync();
        int failed = await _database.Users
            .AsNoTracking()
            .Where(x => x.DiscordId == accountId)
            .CountAsync();

        if (failed > 0)
            throw new DiscordIdAlreadyLinkedException($"The Discord ID {accountId} has already been linked.");

        if (LinkingController.TryGetValue(token, out var old))
        {
            LinkingController[token] = new(token, old.UserId, new Timer((x) =>
                {
                    _ = LinkingController.TryRemove(token, out _);
                },
                null, TimeSpan.FromMinutes(1.5), Timeout.InfiniteTimeSpan), old.Username, old.Password, old.RememberMe,
                    old.LinkSettings, accountId, old.SteamLink, email);

            // Find the next redirect for this account link.

            return LinkingController[token].GetNextRedirect(token);
        }
        else
            throw new TokenNotFoundException("An expired or invalid token was provided.");
    }

    public async Task<string> BindSteamUserAsync(string token, string steamId)
    {
        await using var _database = await _dbContextFactory.CreateDbContextAsync();
        int failed = await _database.Users
            .AsNoTracking()
            .Where(x => x.SteamLink == steamId)
            .CountAsync();

        if (failed > 0)
            throw new FafIdAlreadyLinkedException($"The ID {steamId} for steam is already linked.");

        if (LinkingController.TryGetValue(token, out var old))
        {
            LinkingController[token] = new(token, old.UserId, new Timer((x) =>
                {
                    _ = LinkingController.TryRemove(token, out _);
                },
                null, TimeSpan.FromMinutes(1.5), Timeout.InfiniteTimeSpan), old.Username, old.Password, old.RememberMe,
                    old.LinkSettings, old.DiscordId, steamId, old.DiscordEmail);

            // Find the the next link for this redirect.

            return LinkingController[token].GetNextRedirect(token);
        }
        else
            throw new TokenNotFoundException("An expired or invalid token was provided.");
    }

    public async Task AbortLinkAsync(string token)
    {
        if (LinkingController.TryRemove(token, out var state))
        {
            await state.ExparationTimer.DisposeAsync();
        }
    }

    public (LinkStatus, string?) GetLinkStatus(string token)
    {
        if (LinkingController.TryGetValue(token, out var link))
        {
            if (link.LinkReady)
                return (LinkStatus.Ready, null);
            else return (LinkStatus.Waiting, link.GetNextRedirect(token));
        }

        return (LinkStatus.Invalid, null);
    }

    public async Task<(string, string, bool)> FinalizeLink(string token)
    {
        if (LinkingController.TryRemove(token, out var state))
        {
            var disp = state.ExparationTimer.DisposeAsync();

            await using var _database = await _dbContextFactory.CreateDbContextAsync();
            var user = await _database.FindAsync<DataCoreUser>(state.UserId);

            if (user is null)
                throw new Exception("No user to bind link to.");

            user.DiscordId = state.DiscordId;
            user.SteamLink = state.SteamLink;
            user.Email = state.DiscordEmail;
            user.EmailConfirmed = user.Email is not null;

            await _database.SaveChangesAsync();

            await disp;

            return (state.Username, state.Password, state.RememberMe);
        }
        else
            throw new TokenNotFoundException("An expired or invalid token was provided.");
    }

    /// <summary>
    /// Thrown when a Discord User ID is attempted to be linked to a new FAF account after it has already been linked.
    /// </summary>
    public class DiscordIdAlreadyLinkedException : Exception
    {
        public DiscordIdAlreadyLinkedException() : base() { }
        public DiscordIdAlreadyLinkedException(string? message) : base(message) { }
        public DiscordIdAlreadyLinkedException(string? message, Exception? innerException) : base(message, innerException) { }
        public DiscordIdAlreadyLinkedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when a Faf User ID is attempted to be linked to a Discord account after it has already been linked.
    /// </summary>
    public class FafIdAlreadyLinkedException : Exception
    {
        public FafIdAlreadyLinkedException() : base() { }
        public FafIdAlreadyLinkedException(string? message) : base(message) { }
        public FafIdAlreadyLinkedException(string? message, Exception? innerException) : base(message, innerException) { }
        public FafIdAlreadyLinkedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when a bind is attempted to be made with a token that is no longer valid.
    /// </summary>s
    public class TokenNotFoundException : Exception
    {
        public TokenNotFoundException() : base() { }
        public TokenNotFoundException(string? message) : base(message) { }
        public TokenNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
        public TokenNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
