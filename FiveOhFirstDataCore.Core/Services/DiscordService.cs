using DSharpPlus;
using DSharpPlus.Entities;
using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Discord;
using FiveOhFirstDataCore.Data.Structures.Updates;
using FiveOhFirstDataCore.Data.Structuresbase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class DiscordService : IDiscordService
    {
        private class UpdateDetails
        {
            public HashSet<ulong> ToAdd { get; set; } = new();
            public HashSet<ulong> ToRemove { get; set; } = new();
            public int Id { get; set; } = 0;
        }

        private readonly DiscordRestClient _rest;
        private readonly DiscordClient _client;
        private readonly DiscordBotConfiguration _discordConfig;
        private readonly IWebsiteSettingsService _settings;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        private ConcurrentQueue<(ulong, ulong, bool)> RoleChanges { get; init; } = new();
        private ConcurrentDictionary<ulong, UpdateDetails> UpdateMessages { get; init; } = new();
        private Timer ChangeTimer { get; init; }

        private DiscordGuild HomeGuild { get; set; }


        public DiscordService(DiscordRestClient rest, DiscordClient client, DiscordBotConfiguration discordConfig,
            IWebsiteSettingsService settings, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _rest = rest;
            _client = client;
            _discordConfig = discordConfig;
            _settings = settings;
            _dbContextFactory = dbContextFactory;
            ChangeTimer = new Timer(async (x) => await DoRoleChange(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public async Task InitalizeAsync()
        {
            await _client.ConnectAsync();

            HomeGuild = await _client.GetGuildAsync(_discordConfig.HomeGuild);
        }

        private async Task<string> ConvertMessageAsync(string msg, ulong user, int userId, HashSet<ulong> add, HashSet<ulong> remove)
        {
            if (HomeGuild is null) return msg;

            msg = msg.Replace("{{USER}}", $"<@{user}>");
                
            List<string> data = new();
            foreach(var id in add)
            {
                if(HomeGuild.Roles.TryGetValue(id, out var role))
                {
                    data.Add(role.Name);
                }
            }

            msg = msg.Replace("{{ROLESADDED}}", string.Join(", ", data));

            data = new();
            foreach (var id in add)
                data.Add($"<@{id}>");

            msg = msg.Replace("{{MENTIONROLESADDED}}", string.Join(", ", data));
            
            data = new();
            foreach (var id in remove)
            {
                if (HomeGuild.Roles.TryGetValue(id, out var role))
                {
                    data.Add(role.Name);
                }
            }

            msg = msg.Replace("{{ROLESREMOVED}}", string.Join(", ", data));
            
            data = new();
            foreach (var id in add)
                data.Add($"<@{id}>");

            msg = msg.Replace("{{MENTIONROLESREMOVED}}", string.Join(", ", data));

            await using var _dbContext = _dbContextFactory.CreateDbContext();
                    
            var actual = await _dbContext.FindAsync<Trooper>(userId);
            msg = msg.Replace("{{ID}}", actual?.BirthNumber.ToString() ?? "n/a");
            msg = msg.Replace("{{BIRTHNUMBER}}", actual?.BirthNumber.ToString() ?? "n/a");
            msg = msg.Replace("{{RANK}}", actual?.GetRankName() ?? "n/a");

            return msg;
        }

        private async Task DoRoleChange()
        {
            while (RoleChanges.TryDequeue(out var change))
            {
                if (change.Item1 == 0 || change.Item2 == 0) continue;

                try
                {
                    if (change.Item3)
                    {
                        await _rest.AddGuildMemberRoleAsync(_discordConfig.HomeGuild, change.Item1, change.Item2, "501st Auto Tag Grant");
                    }
                    else
                    {
                        await _rest.RemoveGuildMemberRoleAsync(_discordConfig.HomeGuild, change.Item1, change.Item2, "501st Auto Tag Revoke");
                    }
                }
                catch (Exception ex)
                {
                    _rest.Logger.LogError(ex, $"Failed to update roles for {change.Item1}");
                }

                await PostDiscordMessageAsync(change.Item1);
            }

            ChangeTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private async Task PostDiscordMessageAsync(ulong changeFor)
        {
            if (!UpdateMessages.TryRemove(changeFor, out var messageDetails))
                return;

            if (HomeGuild is null) return;

            var action = await _settings.GetDiscordPostActionConfigurationAsync(DiscordAction.TagUpdate);
            if (action is null) return;
                
            var msg = await ConvertMessageAsync(action.RawMessage ?? "", changeFor, messageDetails.Id, messageDetails.ToAdd, messageDetails.ToRemove);

            DiscordMessageBuilder builder = new();
            builder.WithContent(msg);

            try
            {
                var channel = HomeGuild.GetChannel(action.DiscordChannel);
                await channel.SendMessageAsync(builder);
            }
            catch (Exception ex)
            {
                _rest.Logger.LogError(ex, $"Failed to post role updates for {changeFor}");
            }
        }

        public async Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor, int changeForId)
        {
            if (changeFor == 0) return;

            HashSet<ulong> addU = new();
            foreach (var claim in add)
            {
                var ids = await GetCShopIdAsync(claim);

                if (ids is null) continue;
                foreach (var id in ids)
                {
                    RoleChanges.Enqueue((changeFor, id, true));
                    addU.Add(id);
                }
            }

            HashSet<ulong> removeU = new();
            foreach (var claim in remove)
            {
                var ids = await GetCShopIdAsync(claim);

                if (ids is null) continue;
                foreach (var id in ids)
                {
                    RoleChanges.Enqueue((changeFor, id, false));
                    removeU.Add(id);
                }
            }

            UpdateMessageDetails(addU, removeU, changeFor, changeForId);

            ChangeTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        private async Task<IReadOnlyList<ulong>?> GetCShopIdAsync(Claim value)
            => await _settings.GetCShopDiscordRolesAsync(value);

        public async Task UpdateQualificationChangeAsync(QualificationUpdate change, ulong changeFor, int changeForId)
        {
            if (changeFor == 0) return;
            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            var qType = typeof(Qualification);
            foreach (Qualification qual in Enum.GetValues(qType))
            {
                if ((change.Added & qual) == qual)
                {
                    var grants = await GetAddDelSets(qual, true);
                    add.UnionWith(grants.Item1);
                    del.UnionWith(grants.Item2);
                }

                if ((change.Removed & qual) == qual)
                {
                    var grants = await GetAddDelSets(qual, false);
                    add.UnionWith(grants.Item1);
                    del.UnionWith(grants.Item2);
                }
            }

            add.ExceptWith(del);

            foreach (var v in add)
                RoleChanges.Enqueue((changeFor, v, true));
            foreach (var v in del)
                RoleChanges.Enqueue((changeFor, v, false));

            UpdateMessageDetails(add, del, changeFor, changeForId);

            ChangeTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        public async Task UpdateRankChangeAsync(RankUpdate change, ulong changeFor, int changeForId)
        {
            if (changeFor == 0
                || change.ChangedFrom == 0
                || change.ChangedTo == 0) return;

            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            var from = change.ChangedFrom.GetRank();
            var to = change.ChangedTo.GetRank();

            if (from is null || to is null) return;

            var oldGrants = await GetAddDelSets(from, false);
            var newGrants = await GetAddDelSets(to, true);

            oldGrants.Item1.UnionWith(newGrants.Item1);
            add.UnionWith(oldGrants.Item1);

            oldGrants.Item2.UnionWith(newGrants.Item2);
            del.UnionWith(oldGrants.Item2);

            add.ExceptWith(del);

            foreach (var v in add)
                RoleChanges.Enqueue((changeFor, v, true));
            foreach (var v in del)
                RoleChanges.Enqueue((changeFor, v, false));

            UpdateMessageDetails(add, del, changeFor, changeForId);

            ChangeTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        public async Task UpdateSlotChangeAsync(SlotUpdate change, ulong changeFor, int changeForId)
        {
            if (changeFor == 0) return;
            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            if (change.NewSlot != change.OldSlot)
            {
                var oldGrants = await GetAddDelSets(change.OldSlot, false);
                var newGrants = await GetAddDelSets(change.NewSlot, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            if (change.NewFlight != change.OldFlight
                && change.NewFlight is not null
                && change.OldFlight is not null)
            {
                var oldGrants = await GetAddDelSets(change.OldFlight, false);
                var newGrants = await GetAddDelSets(change.NewFlight, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            if (change.NewRole != change.OldRole
                && change.NewRole is not null
                && change.OldRole is not null)
            {
                var oldGrants = await GetAddDelSets(change.OldRole, false);
                var newGrants = await GetAddDelSets(change.NewRole, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            if (change.NewTeam != change.OldTeam
                && change.NewTeam is not null
                && change.OldTeam is not null)
            {
                var oldGrants = await GetAddDelSets(change.OldTeam, false);
                var newGrants = await GetAddDelSets(change.NewTeam, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            add.ExceptWith(del);

            foreach (var v in add)
                RoleChanges.Enqueue((changeFor, v, true));
            foreach (var v in del)
                RoleChanges.Enqueue((changeFor, v, false));

            UpdateMessageDetails(add, del, changeFor, changeForId);

            ChangeTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        private async Task<(HashSet<ulong>, HashSet<ulong>)> GetAddDelSets(Enum val, bool addVal)
        {
            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            var ids = await _settings.GetDiscordRoleDetailsAsync(val);

            if (ids is not null)
            {
                foreach (var a in ids.RoleGrants)
                    if (addVal) add.Add(a);
                    else del.Add(a);
                foreach (var d in ids.RoleReplaces)
                    if (addVal) del.Add(d);
                    else add.Add(d);
            }

            return (add, del);
        }

        private void UpdateMessageDetails(HashSet<ulong> add, HashSet<ulong> del, ulong user, int userId)
        {
            if(UpdateMessages.TryGetValue(user, out var details))
            {
                details.ToAdd.UnionWith(add);
                details.ToRemove.UnionWith(del);
                details.Id = userId;
            }
            else
            {
                details = new()
                {
                    ToAdd = add,
                    ToRemove = del,
                    Id = userId
                };
                UpdateMessages[user] = details;
            }
        }

        public async Task<IReadOnlyList<DiscordRole>> GetAllHomeGuildRolesAsync()
        {
            var roles = await _rest.GetGuildRolesAsync(_discordConfig.HomeGuild);
            return roles;
        }

        public async Task<IReadOnlyList<DiscordChannel>> GetAllHomeGuildChannelsAsync()
        {
            var channels = await _rest.GetGuildChannelsAsync(_discordConfig.HomeGuild);
            return channels;
        }
    }
}
