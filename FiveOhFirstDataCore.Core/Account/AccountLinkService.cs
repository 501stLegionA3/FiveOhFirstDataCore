using FiveOhFirstDataCore.Data.Structuresbase;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace FiveOhFirstDataCore.Data.Account
{
    public class AccountLinkService
    {
        public enum LinkStatus
        {
            Ready,
            Waiting,
            Invalid
        }

        public ConcurrentDictionary<string, LinkState> LinkingController { get; init; }
        private readonly IServiceProvider _services;

        public AccountLinkService(IServiceProvider services)
        {
            _services = services;
            LinkingController = new();
        }

        public async Task<string> StartAsync(int trooperId, string username, string password, bool rememberMe)
        {
            var scope = _services.CreateScope();
            var _databaseFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var _database = _databaseFac.CreateDbContext();
            var trooper = await _database.FindAsync<Trooper>(trooperId);
            if (trooper is null)
                if (trooper?.DiscordId is not null)
                    throw new DiscordIdAlreadyLinkedException($"The trooper {trooperId} has already linked to the Discord account {trooper.DiscordId}");
                else
                    throw new Exception($"The trooper is null.");

            var token = Guid.NewGuid().ToString();

            LinkingController[token] = new(token, trooperId, new Timer((x) =>
            {
                _ = LinkingController.TryRemove(token, out _);
            },
            null, TimeSpan.FromMinutes(1.5), Timeout.InfiniteTimeSpan), username, password, rememberMe);

            return token;
        }

        public async Task BindDiscordAsync(string token, ulong accountId, string email)
        {
            var scope = _services.CreateScope();
            var _databaseFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var _database = _databaseFac.CreateDbContext();
            int failed = 0;
            await _database.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.DiscordId is not null && x.DiscordId == accountId.ToString())
                    failed += 1;
            });

            if (failed > 0)
                throw new DiscordIdAlreadyLinkedException($"The Discord ID {accountId} has already been linked.");

            if (LinkingController.TryGetValue(token, out var old))
            {
                LinkingController[token] = new(token, old.TrooperId, accountId.ToString(), email, new Timer((x) =>
                {
                    _ = LinkingController.TryRemove(token, out _);
                }, null, TimeSpan.FromMinutes(1.5), Timeout.InfiniteTimeSpan), old.Username, old.Password, old.RememberMe);
            }
            else
                throw new TokenNotFoundException("An expired or invalid token was provided.");
        }

        public async Task BindSteamUserAsync(string token, string steamId)
        {
            var scope = _services.CreateScope();
            var _databaseFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var _database = _databaseFac.CreateDbContext();
            int failed = 0;
            await _database.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.SteamLink is not null && x.SteamLink == steamId)
                    failed += 1;
            });

            if (failed > 0)
                throw new FafIdAlreadyLinkedException($"The ID {steamId} for steam is already linked.");

            if (LinkingController.TryGetValue(token, out var old))
            {
                if (old.DiscordId is null)
                    throw new Exception("Discord ID is missing.");

                LinkingController[token] = new(token, old.TrooperId, old.DiscordId, old.DiscordEmail,
                    steamId, new Timer((x) =>
                    {
                        _ = LinkingController.TryRemove(token, out _);
                    },
                    null, TimeSpan.FromMinutes(1.5), Timeout.InfiniteTimeSpan), old.Username, old.Password, old.RememberMe);
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

        public LinkStatus GetLinkStatus(string token)
        {
            if (LinkingController.TryGetValue(token, out var link))
            {
                if (link.LinkReady)
                    return LinkStatus.Ready;
                else return LinkStatus.Waiting;
            }

            return LinkStatus.Invalid;
        }

        public async Task<(string, string, bool)> FinalizeLink(string token)
        {
            if (LinkingController.TryRemove(token, out var state))
            {
                var disp = state.ExparationTimer.DisposeAsync();

                var scope = _services.CreateScope();
                var _databaseFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                using var _database = _databaseFac.CreateDbContext();
                var user = await _database.FindAsync<Trooper>(state.TrooperId);

                if (user is null)
                    throw new Exception("No user to bind link to.");

                user.DiscordId = state.DiscordId;
                user.SteamLink = state.SteamId;
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
}
