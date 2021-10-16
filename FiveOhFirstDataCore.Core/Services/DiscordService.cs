using DSharpPlus;
using DSharpPlus.Entities;
using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Discord;
using FiveOhFirstDataCore.Data.Structures.Updates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class DiscordService : IDiscordService
    {
        private class UpdateDetails
        {
            public List<ulong> ToAdd { get; set; } = new();
            public List<ulong> ToRemove { get; set; } = new();
            public int Id { get; set; } = 0;
        }

        private readonly DiscordRestClient _rest;
        private readonly DiscordClient _client;
        private readonly DiscordBotConfiguration _discordConfig;
        private readonly IWebsiteSettingsService _settings;
        private readonly UserManager<Trooper> _manager;

        private ConcurrentQueue<(ulong, ulong, bool)> RoleChanges { get; init; } = new();
        private ConcurrentDictionary<ulong, UpdateDetails> UpdateMessages { get; init; } = new();
        private Timer ChangeTimer { get; init; }

        private DiscordGuild HomeGuild { get; set; }


        public DiscordService(DiscordRestClient rest, DiscordClient client, DiscordBotConfiguration discordConfig,
            IWebsiteSettingsService settings, UserManager<Trooper> manager)
        {
            _rest = rest;
            _client = client;
            _discordConfig = discordConfig;
            _settings = settings;
            _manager = manager;
            ChangeTimer = new Timer(async (x) => await DoRoleChange(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public async Task InitalizeAsync()
        {
            HomeGuild = await _client.GetGuildAsync(_discordConfig.HomeGuild);
        }

        private async Task<string> ConvertTokenAsync(string token, ulong user, int userId, List<ulong> add, List<ulong> remove)
        {
            switch(token.ToUpper())
            {
                case "{{USER}}":
                    return $"<@{user}>";
                case "{{ROLESADDED}}":
                    List<string> data = new();
                    foreach(var id in add)
                    {
                        if(HomeGuild.Roles.TryGetValue(id, out var role))
                        {
                            data.Add(role.Name);
                        }
                    }

                    return string.Join(", ", data);
                case "{{MENTIONROLESADDED}}":
                    data = new();
                    foreach (var id in add)
                        data.Add($"<@{id}>");

                    return string.Join(", ", data);
                case "{{ROLESREMOVED}}":
                    data = new();
                    foreach (var id in remove)
                    {
                        if (HomeGuild.Roles.TryGetValue(id, out var role))
                        {
                            data.Add(role.Name);
                        }
                    }

                    return string.Join(", ", data);
                case "{{MENTIONROLESREMOVED}}":
                    data = new();
                    foreach (var id in add)
                        data.Add($"<@{id}>");

                    return string.Join(", ", data);
                case "{{ID}}":
                case "{{BIRTHNUMBER}}":
                    var actual = await _manager.FindByIdAsync(userId.ToString());
                    return actual?.BirthNumber.ToString() ?? "n/a";
                case "{{RANK}}":
                    actual = await _manager.FindByIdAsync(userId.ToString());
                    return actual?.GetRankName() ?? "n/a";
                default:
                    return token;
            }
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

        public async Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor, int changeForId)
        {
            if (changeFor == 0) return;

            foreach (var claim in add)
            {
                var ids = await GetCShopIdAsync(claim);

                if (ids is null) continue;
                foreach (var id in ids)
                    RoleChanges.Enqueue((changeFor, id, true));
            }

            foreach (var claim in remove)
            {
                var ids = await GetCShopIdAsync(claim);

                if (ids is null) continue;
                foreach (var id in ids)
                    RoleChanges.Enqueue((changeFor, id, false));
            }

            ChangeTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        private async Task PostDiscordMessageAsync(ulong changeFor)
        {
            if (!UpdateMessages.TryRemove(changeFor, out var messageDetails))
                return;

            var action = await _settings.GetDiscordPostActionConfigurationAsync(DiscordAction.TagUpdate);
            if (action is null) return;

            List<string> parts = new();
            foreach (var c in action.RawMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                parts.Add(await ConvertTokenAsync(c, changeFor, messageDetails.Id, messageDetails.ToAdd, messageDetails.ToRemove));

            string msg = string.Join(" ", parts.ToArray());

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
