using DSharpPlus;

using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Core.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly DiscordRestClient _rest;
        private readonly DiscordBotConfiguration _discordConfig;
        private readonly IWebsiteSettingsService _settings;

        private ConcurrentQueue<(ulong, ulong, bool)> RoleChanges { get; init; } = new();
        private Timer ChangeTimer { get; init; }

        public DiscordService(DiscordRestClient rest, DiscordBotConfiguration discordConfig,
            IWebsiteSettingsService settings)
        {
            _rest = rest;
            _discordConfig = discordConfig;
            _settings = settings;
            ChangeTimer = new Timer(async (x) => await DoRoleChange(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
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
            }

            ChangeTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public async Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor)
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

        private async Task<IReadOnlyList<ulong>?> GetCShopIdAsync(Claim value)
            => await _settings.GetCShopDiscordRolesAsync(value);

        public async Task UpdateQualificationChangeAsync(QualificationUpdate change, ulong changeFor)
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

        public async Task UpdateRankChangeAsync(RankUpdate change, ulong changeFor)
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

        public async Task UpdateSlotChangeAsync(SlotUpdate change, ulong changeFor)
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
    }
}
