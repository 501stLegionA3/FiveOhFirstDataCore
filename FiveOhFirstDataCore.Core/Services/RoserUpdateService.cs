using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public partial class RosterService : IRosterService
    {
        public async Task<Dictionary<CShop, List<ClaimUpdate>>> GetCShopClaimsAsync(Trooper trooper)
        {
            Dictionary<CShop, List<ClaimUpdate>> claimUpdates = new();

            var rawSet = await _userManager.GetClaimsAsync(trooper);

            foreach(var c in rawSet)
            {
                foreach(var shops in CShopExtensions.ClaimsTree)
                {
                    if(shops.Value.ContainsKey(c.Type))
                    {
                        if (claimUpdates.TryGetValue(shops.Key, out var list))
                            list.Add(new(c.Type, c.Value));
                        else claimUpdates.Add(shops.Key, new() { new(c.Type, c.Value) });
                    }
                }
            }

            return claimUpdates;
        }

        public async Task<ResultBase> UpdateAsync(Trooper edit, List<ClaimUpdate> claimsToAdd, 
            List<ClaimUpdate> claimsToRemove, ClaimsPrincipal submitterClaim)
        {
            var primary = await _dbContext.FindAsync<Trooper>(edit.Id);
            var submitter = await _userManager.GetUserAsync(submitterClaim);

            List<string> errors = new();

            if(primary is null)
            {
                errors.Add("No trooper was found.");
                return new(false, errors);
            }

            if(submitter is null)
            {
                errors.Add("No subbiter found.");
                return new(false, errors);
            }

            // Rank updates.
            if (UpdateRank((int)primary.Rank, (int)edit.Rank, ref primary, ref submitter))
                primary.Rank = edit.Rank;

            if (UpdateRank((int?)primary.RTORank, (int?)edit.RTORank, ref primary, ref submitter))
                primary.RTORank = edit.RTORank;

            if (UpdateRank((int?)primary.MedicRank, (int?)edit.MedicRank, ref primary, ref submitter))
                primary.MedicRank = edit.MedicRank;

            if (UpdateRank((int?)primary.PilotRank, (int?)edit.PilotRank, ref primary, ref submitter))
                primary.PilotRank = edit.PilotRank;

            if (UpdateRank((int?)primary.WarrantRank, (int?)edit.WarrantRank, ref primary, ref submitter))
                primary.WarrantRank = edit.WarrantRank;

            if (UpdateRank((int?)primary.WardenRank, (int?)edit.WardenRank, ref primary, ref submitter))
                primary.WardenRank = edit.WardenRank;

            // Slot updates.
            UpdateRosterPosition(edit, ref primary, ref submitter);

            // C-Shop/Qual updates
            UpdateCShop(edit, ref primary, ref submitter);
            UpdateQuals(edit, ref primary, ref submitter);

            primary.InitalTraining = edit.InitalTraining;
            primary.UTC = edit.UTC;

            primary.Notes = edit.Notes;
            // Claim Modification
            List<Claim> remove = new();
            claimsToRemove.ForEach(x =>
            {
                remove.Add(new(x.Key, x.Value));
            });

            var identResult = await _userManager.RemoveClaimsAsync(primary, remove);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            var exsistingClaims = (await GetCShopClaimsAsync(primary)).ToList();

            List<Claim> add = new();
            claimsToAdd.ForEach(x =>
            {
                var exsisting = exsistingClaims.Any(z => z.Value.Any(y => y.Key == x.Key && y.Value == x.Value));

                if (!exsisting)
                {
                    add.Add(new(x.Key, x.Value));
                }
            });

            identResult = await _userManager.AddClaimsAsync(primary, add);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            try
            {
                _dbContext.Update(primary);
                await _dbContext.SaveChangesAsync();
                identResult = await _userManager.UpdateAsync(submitter);

                if(!identResult.Succeeded)
                {
                    foreach (var err in identResult.Errors)
                        errors.Add($"[{err.Code}] {err.Description}");

                    return new(false, errors);
                }

                return new(true, null);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return new(false, errors);
            }
        }

        protected static bool UpdateRank(int? primary, int? edit, ref Trooper p, ref Trooper s)
        {
            if(primary != edit)
            {
                var update = new RankChange()
                {
                    ChangedFrom = primary ?? -1,
                    ChangedTo = edit ?? -1,
                    ChangedOn = DateTime.UtcNow,
                    SubmittedByRosterClerk = true
                };

                p.RankChanges.Add(update);
                s.SubmittedRankChanges.Add(update);

                if(primary < edit)
                {
                    p.LastPromotion = DateTime.UtcNow;
                }

                return true;
            }

            return false;
        }

        protected static void UpdateRosterPosition(Trooper edit, ref Trooper primary, ref Trooper submitter)
        {
            if (primary.Slot != edit.Slot
                || primary.Role != edit.Role
                || primary.Team != edit.Team
                || primary.Flight != edit.Flight)
            {
                var update = new SlotChange()
                {
                    NewSlot = edit.Slot,
                    NewRole = edit.Role,
                    NewTeam = edit.Team,
                    NewFlight = edit.Flight,

                    OldSlot = primary.Slot,
                    OldRole = primary.Role,
                    OldTeam = primary.Team,
                    OldFlight = primary.Flight,

                    ChangedOn = DateTime.UtcNow,
                    SubmittedByRosterClerk = true
                };

                primary.Slot = edit.Slot;
                primary.Role = edit.Role;
                primary.Team = edit.Team;
                primary.Flight = edit.Flight;

                primary.SlotChanges.Add(update);
                submitter.ApprovedSlotChanges.Add(update);
            }
        }

        protected static void UpdateCShop(Trooper edit, ref Trooper primary, ref Trooper submitter)
        {
            if(primary.CShops != edit.CShops)
            {
                var changes = primary.CShops ^ edit.CShops;
                var additions = edit.CShops & changes;
                var removals = primary.CShops & changes;

                var update = new CShopChange()
                {
                    Added = additions,
                    Removed = removals,
                    OldCShops = primary.CShops,
                    
                    SubmittedByRosterClerk = true,
                    ChangedOn = DateTime.UtcNow
                };

                primary.CShops = edit.CShops;

                primary.CShopChanges.Add(update);
                submitter.SubmittedCShopChanges.Add(update);
            }
        }

        protected static void UpdateQuals(Trooper edit, ref Trooper primary, ref Trooper submitter)
        {
            if (primary.Qualifications != edit.Qualifications)
            {
                var changes = primary.Qualifications ^ edit.Qualifications;
                var additions = edit.Qualifications & changes;
                var removals = primary.Qualifications & changes;

                var update = new QualificationChange()
                {
                    Added = additions,
                    Removed = removals,
                    OldQualifications = primary.Qualifications,
                    Revoked = false,

                    SubmittedByRosterClerk = true,
                    ChangedOn = DateTime.UtcNow
                };

                primary.Qualifications = edit.Qualifications;

                primary.QualificationChanges.Add(update);
                submitter.SubmittedQualificationChanges.Add(update);
            }
        }

        public async Task SaveNewFlag(ClaimsPrincipal claim, Trooper trooper, TrooperFlag flag)
        {
            var user = await _userManager.GetUserAsync(claim);
            if (_dbContext.Entry(trooper).State == EntityState.Detached)
                _dbContext.Attach(trooper);

            flag.AuthorId = user.Id;
            flag.CreatedOn = DateTime.UtcNow;

            trooper.Flags.Add(flag);

            await _dbContext.SaveChangesAsync();
        }
    }
}
