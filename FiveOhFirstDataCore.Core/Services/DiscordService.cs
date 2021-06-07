using DSharpPlus;
using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly DiscordRestClient _rest;
        private readonly DiscordBotConfiguration _discordConfig;

        private ConcurrentQueue<(ulong, ulong, bool)> RoleChanges { get; init; } = new();
        private Timer ChangeTimer { get; init; }

        public DiscordService(DiscordRestClient rest, DiscordBotConfiguration discordConfig)
        {
            _rest = rest;
            _discordConfig = discordConfig;
            ChangeTimer = new Timer(async (x) => await DoRoleChange(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private async Task DoRoleChange()
        {
            while(RoleChanges.TryDequeue(out var change))
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

        public Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor)
        {
            if (changeFor == 0) return Task.CompletedTask;

            foreach(var claim in add)
            {
                var ids = GetCShopId(claim);

                if (ids is null) continue;
                foreach(var id in ids)
                    RoleChanges.Enqueue((changeFor, id, true));
            }

            foreach(var claim in remove)
            {
                var ids = GetCShopId(claim);

                if (ids is null) continue;
                foreach(var id in ids)
                    RoleChanges.Enqueue((changeFor, id, false));
            }

            ChangeTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        private ulong[]? GetCShopId(Claim value)
        {
            foreach(var val in _discordConfig.CShopRoleBindings.Values)
            {
                if (val.TryGetValue(value.Type, out var roleSet))
                    if (roleSet.TryGetValue(value.Value, out var roles))
                        return roles;
            }

            return null;
        }

        public Task UpdateQualificationChangeAsync(QualificationUpdate change, ulong changeFor)
        {
            if (changeFor == 0) return Task.CompletedTask;
            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            var qType = typeof(Qualification);
            foreach (Qualification qual in Enum.GetValues(qType))
            {
                if((change.Added & qual) == qual)
                {
                    var grants = GetAddDelSets(qual, true);
                    add.UnionWith(grants.Item1);
                    del.UnionWith(grants.Item2);
                }

                if((change.Removed & qual) == qual)
                {
                    var grants = GetAddDelSets(qual, false);
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

            return Task.CompletedTask;
        }

        public Task UpdateRankChangeAsync(RankUpdate change, ulong changeFor)
        {
            if (changeFor == 0 
                || change.ChangedFrom == 0 
                || change.ChangedTo == 0) return Task.CompletedTask;

            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            var from = change.ChangedFrom.GetRank();
            var to = change.ChangedTo.GetRank();

            if (from is null || to is null) return Task.CompletedTask;

            var oldGrants = GetAddDelSets(from, false);
            var newGrants = GetAddDelSets(to, true);

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

            return Task.CompletedTask;
        }

        public Task UpdateSlotChangeAsync(SlotUpdate change, ulong changeFor)
        {
            if (changeFor == 0) return Task.CompletedTask;
            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            if(change.NewSlot != change.OldSlot)
            {
                var oldGrants = GetAddDelSets(change.OldSlot, false);
                var newGrants = GetAddDelSets(change.NewSlot, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            if(change.NewFlight != change.OldFlight
                && change.NewFlight is not null
                && change.OldFlight is not null)
            {
                var oldGrants = GetAddDelSets(change.OldFlight, false);
                var newGrants = GetAddDelSets(change.NewFlight, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            if (change.NewRole != change.OldRole
                && change.NewRole is not null
                && change.OldRole is not null)
            {
                var oldGrants = GetAddDelSets(change.OldRole, false);
                var newGrants = GetAddDelSets(change.NewRole, true);

                oldGrants.Item1.UnionWith(newGrants.Item1);
                add.UnionWith(oldGrants.Item1);

                oldGrants.Item2.UnionWith(newGrants.Item2);
                del.UnionWith(oldGrants.Item2);
            }

            if (change.NewTeam != change.OldTeam
                && change.NewTeam is not null
                && change.OldTeam is not null)
            {
                var oldGrants = GetAddDelSets(change.OldTeam, false);
                var newGrants = GetAddDelSets(change.NewTeam, true);

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

            return Task.CompletedTask;
        }

        private (HashSet<ulong>, HashSet<ulong>) GetAddDelSets(Enum val, bool addVal)
        {
            HashSet<ulong> add = new();
            HashSet<ulong> del = new();

            var name = val.GetType().Name;
            var ids = GetDiscordIds(name, val.ToString());

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

        private DiscordRoleDetails? GetDiscordIds(string group, string value)
        {
            if (_discordConfig.RoleBindings.TryGetValue(group, out var sets))
                if (sets.TryGetValue(value, out var details))
                    return details;

            return null;
        }
    }
}
