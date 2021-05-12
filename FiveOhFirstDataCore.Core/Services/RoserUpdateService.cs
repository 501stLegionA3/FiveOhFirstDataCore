using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
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
        public async Task<ResultBase> UpdateAsync(Trooper edit, ClaimsPrincipal submitterClaim)
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

            try
            {
                _dbContext.Update(primary);
                await _dbContext.SaveChangesAsync();
                var identResult = await _userManager.UpdateAsync(submitter);

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

                primary.CShops = edit.CShops;

                primary.QualificationChanges.Add(update);
                submitter.SubmittedQualificationChanges.Add(update);
            }
        }
    }
}
