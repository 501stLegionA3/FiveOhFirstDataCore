using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Updates;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public partial class RosterService : IRosterService
    {
        public async Task<Dictionary<CShop, List<ClaimUpdateData>>> GetCShopClaimsAsync(Trooper trooper)
        {
            Dictionary<CShop, List<ClaimUpdateData>> claimUpdates = new();

            var rawSet = await _userManager.GetClaimsAsync(trooper);

            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            foreach (var c in rawSet)
            {
                foreach (var shops in ClaimsTree)
                {
                    if (shops.Value.ClaimData.ContainsKey(c.Type))
                    {
                        if (claimUpdates.TryGetValue(shops.Key, out var list))
                            list.Add(new(c.Type, c.Value));
                        else claimUpdates.Add(shops.Key, new() { new(c.Type, c.Value) });
                    }
                }
            }

            return claimUpdates;
        }

        public async Task<ResultBase> UpdateAsync(Trooper edit, List<ClaimUpdateData> claimsToAdd,
            List<ClaimUpdateData> claimsToRemove, ClaimsPrincipal submitterClaim)
        {
            var primary = await _userManager.FindByIdAsync(edit.Id.ToString());
            Slot oldSlot = primary.Slot;
            var submitter = await _userManager.GetUserAsync(submitterClaim);

            List<string> errors = new();

            if (primary is null)
            {
                errors.Add("No trooper was found.");
                return new(false, errors);
            }

            if (submitter is null)
            {
                errors.Add("No subbiter found.");
                return new(false, errors);
            }

            _ = ulong.TryParse(primary.DiscordId, out ulong pid);

            // Rank updates.
            if (UpdateRank((int?)primary.Rank, (int?)edit.Rank, ref primary, ref submitter, out var rankChange))
            {
                primary.Rank = edit.Rank;
            }

            if (UpdateRank((int?)primary.RTORank, (int?)edit.RTORank, ref primary, ref submitter, out rankChange))
            {
                primary.RTORank = edit.RTORank;
            }

            if (UpdateRank((int?)primary.MedicRank, (int?)edit.MedicRank, ref primary, ref submitter, out rankChange))
            {
                primary.MedicRank = edit.MedicRank;
            }

            if (UpdateRank((int?)primary.PilotRank, (int?)edit.PilotRank, ref primary, ref submitter, out rankChange))
            {
                primary.PilotRank = edit.PilotRank;
            }

            if (UpdateRank((int?)primary.WarrantRank, (int?)edit.WarrantRank, ref primary, ref submitter, out rankChange))
            {
                primary.WarrantRank = edit.WarrantRank;
            }

            if (UpdateRank((int?)primary.WardenRank, (int?)edit.WardenRank, ref primary, ref submitter, out rankChange))
            {
                primary.WardenRank = edit.WardenRank;
            }

            // Time Updates
            TimeUpdate? timeUpdate = null;

            if (!primary.GraduatedBCTOn.Equals(edit.GraduatedBCTOn))
            {
                if (timeUpdate is null)
                    timeUpdate = new();

                timeUpdate.OldGraduatedBCT = primary.GraduatedBCTOn;
                timeUpdate.NewGraduatedBCT = edit.GraduatedBCTOn;

                primary.GraduatedBCTOn = edit.GraduatedBCTOn;
            }

            if (!primary.GraduatedUTCOn.Equals(edit.GraduatedUTCOn))
            {
                if (timeUpdate is null)
                    timeUpdate = new();

                timeUpdate.OldGraduatedUTC = primary.GraduatedUTCOn;
                timeUpdate.NewGraduatedUTC = edit.GraduatedUTCOn;

                primary.GraduatedUTCOn = edit.GraduatedUTCOn;
            }

            if (!primary.LastBilletChange.Equals(edit.LastBilletChange))
            {
                if (timeUpdate is null)
                    timeUpdate = new();

                timeUpdate.OldBilletChange = primary.LastBilletChange;
                timeUpdate.NewBilletChange = edit.LastBilletChange;
                primary.LastBilletChange = edit.LastBilletChange;

            }

            if (!primary.LastPromotion.Equals(edit.LastPromotion))
            {
                if (timeUpdate is null)
                    timeUpdate = new();

                timeUpdate.OldPromotion = primary.LastPromotion;
                timeUpdate.NewPromotion = edit.LastPromotion;

                primary.LastPromotion = edit.LastPromotion;
            }

            if (!primary.StartOfService.Equals(edit.StartOfService))
            {
                if (timeUpdate is null)
                    timeUpdate = new();

                timeUpdate.OldStartOfService = primary.StartOfService;
                timeUpdate.NewStartOfService = edit.StartOfService;

                primary.StartOfService = edit.StartOfService;
            }

            // Save time update
            if (timeUpdate is not null)
            {
                timeUpdate.ChangedById = submitter.Id;
                timeUpdate.ChangedOn = DateTime.UtcNow.ToEst();
                timeUpdate.SubmittedByRosterClerk = true;

                primary.TimeUpdates.Add(timeUpdate);
            }

            // MP Update
            if (primary.MilitaryPolice != edit.MilitaryPolice)
            {
                primary.MilitaryPolice = edit.MilitaryPolice;

                // Save MP Changes
                var mprole = WebsiteRoles.MP.ToString();
                IdentityResult? identRes;
                if (edit.MilitaryPolice)
                {
                    identRes = await _userManager.AddToRoleAsync(primary, mprole);
                }
                else
                {
                    identRes = await _userManager.RemoveFromRoleAsync(primary, mprole);
                }

                if (!identRes.Succeeded)
                {
                    foreach (var err in identRes.Errors)
                        errors.Add($"[{err.Code}] {err.Description}");

                    return new(false, errors);
                }
            }

            // Slot updates.
            _ = UpdateRosterPosition(edit, ref primary, ref submitter, out var slotChange);

            // C-Shop/Qual updates
            _ = UpdateCShop(edit, ref primary, ref submitter, out _);
            _ = UpdateQuals(edit, ref primary, ref submitter, out var qualChange);

            primary.InitialTraining = edit.InitialTraining;
            primary.UTC = edit.UTC;

            primary.Notes = edit.Notes;
            // Claim Modification
            List<Claim> remove = new();

            var existingClaims = (await GetCShopClaimsAsync(primary)).ToList();
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

            List<Claim> add = new();
            claimsToAdd.ForEach(x =>
            {
                var existing = existingClaims.Any(z => z.Value.Any(y => y.Key == x.Key && y.Value == x.Value));

                if (!existing)
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

            primary.LastUpdate = DateTime.UtcNow;

            try
            {
                identResult = await _userManager.UpdateAsync(primary);

                if (!identResult.Succeeded)
                {
                    foreach (var err in identResult.Errors)
                        errors.Add($"[{err.Code}] {err.Description}");

                    return new(false, errors);
                }

                identResult = await _userManager.UpdateAsync(submitter);

                if (!identResult.Succeeded)
                {
                    foreach (var err in identResult.Errors)
                        errors.Add($"[{err.Code}] {err.Description}");

                    return new(false, errors);
                }

                if (primary.Slot == Slot.Archived)
                {
                    identResult = await _userManager.AddToRoleAsync(primary, "Archived");

                    if (!identResult.Succeeded)
                    {
                        foreach (var err in identResult.Errors)
                            errors.Add($"[{err.Code}] {err.Description}");

                        return new(false, errors);
                    }
                }
                else if (primary.Slot != Slot.Archived
                    && oldSlot == Slot.Archived)
                {
                    identResult = await _userManager.RemoveFromRoleAsync(primary, "Archived");

                    if (!identResult.Succeeded)
                    {
                        foreach (var err in identResult.Errors)
                            errors.Add($"[{err.Code}] {err.Description}");

                        return new(false, errors);
                    }
                }

                return new(true, null);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return new(false, errors);
            }
        }

        protected static bool UpdateRank(int? primary, int? edit, ref Trooper p, ref Trooper s,
            [NotNullWhen(true)] out RankUpdate? update)
        {
            if (primary != edit)
            {
                update = new RankUpdate()
                {
                    ChangedFrom = primary ?? -1,
                    ChangedTo = edit ?? -1,
                    ChangedOn = DateTime.UtcNow.ToEst(),
                    SubmittedByRosterClerk = true
                };

                p.RankChanges.Add(update);
                s.SubmittedRankUpdates.Add(update);

                return true;
            }

            update = null;
            return false;
        }

        protected static bool UpdateRosterPosition(Trooper edit, ref Trooper primary, ref Trooper submitter,
            [NotNullWhen(true)] out SlotUpdate? update)
        {
            if (primary.Slot != edit.Slot
                || primary.Role != edit.Role
                || primary.Team != edit.Team
                || primary.Flight != edit.Flight)
            {
                update = new SlotUpdate()
                {
                    NewSlot = edit.Slot,
                    NewRole = edit.Role,
                    NewTeam = edit.Team,
                    NewFlight = edit.Flight,

                    OldSlot = primary.Slot,
                    OldRole = primary.Role,
                    OldTeam = primary.Team,
                    OldFlight = primary.Flight,

                    ChangedOn = DateTime.UtcNow.ToEst(),
                    SubmittedByRosterClerk = true
                };

                primary.Slot = edit.Slot;
                primary.Role = edit.Role;
                primary.Team = edit.Team;
                primary.Flight = edit.Flight;

                primary.SlotUpdates.Add(update);
                submitter.ApprovedSlotUpdates.Add(update);

                return true;
            }

            update = null;
            return false;
        }

        protected static bool UpdateCShop(Trooper edit, ref Trooper primary, ref Trooper submitter,
            [NotNullWhen(true)] out CShopUpdate? update)
        {
            if (primary.CShops != edit.CShops)
            {
                var changes = primary.CShops ^ edit.CShops;
                var additions = edit.CShops & changes;
                var removals = primary.CShops & changes;

                update = new CShopUpdate()
                {
                    Added = additions,
                    Removed = removals,
                    OldCShops = primary.CShops,

                    SubmittedByRosterClerk = true,
                    ChangedOn = DateTime.UtcNow.ToEst()
                };

                primary.CShops = edit.CShops;

                primary.CShopUpdates.Add(update);
                submitter.SubmittedCShopUpdates.Add(update);

                return true;
            }

            update = null;
            return false;
        }

        protected static bool UpdateQuals(Trooper edit, ref Trooper primary, ref Trooper submitter,
            [NotNullWhen(true)] out QualificationUpdate? update)
        {
            if (primary.Qualifications != edit.Qualifications)
            {
                var changes = primary.Qualifications ^ edit.Qualifications;
                var additions = edit.Qualifications & changes;
                var removals = primary.Qualifications & changes;

                update = new QualificationUpdate()
                {
                    Added = additions,
                    Removed = removals,
                    OldQualifications = primary.Qualifications,
                    Revoked = false,

                    SubmittedByRosterClerk = true,
                    ChangedOn = DateTime.UtcNow.ToEst()
                };

                primary.Qualifications = edit.Qualifications;

                primary.QualificationUpdates.Add(update);
                submitter.SubmittedQualificationUpdates.Add(update);

                return true;
            }

            update = null;
            return false;
        }

        public async Task SaveNewFlag(ClaimsPrincipal claim, Trooper trooper, TrooperFlag flag)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var user = await _userManager.GetUserAsync(claim);
            if (_dbContext.Entry(trooper).State == EntityState.Detached)
                _dbContext.Attach(trooper);

            flag.AuthorId = user.Id;
            flag.CreatedOn = DateTime.UtcNow.ToEst();

            trooper.Flags.Add(flag);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<ResultBase> SaveNewDescription(ClaimsPrincipal claim, Trooper trooper, TrooperDescription description)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var user = await _userManager.GetUserAsync(claim);
            var t = await _dbContext.FindAsync<Trooper>(trooper.Id);
            if (t is null)
                return new(false, new List<string> { "Trooper Not Found" });
            await _dbContext.Entry(t).Collection(e => e.Descriptions).LoadAsync();
            //Console.WriteLine(t.Descriptions.Count);

            description.AuthorId = user.Id;
            description.CreatedOn = DateTime.UtcNow.ToEst();
            description.Order = t.Descriptions.Count;

            t.Descriptions.Add(description);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public async Task<ResultBase> UpdateDescriptionOrderAsync(Trooper trooper, TrooperDescription desc, int index)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var t = await _dbContext.FindAsync<Trooper>(trooper.Id);
            if (t is null)
                return new(false, new List<string> { "Trooper Not Found" });
            await _dbContext.Entry(t).Collection(e => e.Descriptions).LoadAsync();
            t.Descriptions.Sort((x, y) => x.Order.CompareTo(y.Order));
            var d = await _dbContext.FindAsync<TrooperDescription>(desc.Id);

            t.Descriptions.RemoveAt(d.Order);
            t.Descriptions.Insert(index, d);
            for (int i = 0; i < t.Descriptions.Count; i++)
            {
                t.Descriptions[i].Order = i;
            }

            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }

        public async Task<ResultBase> DeleteDescriptionAsync(Trooper trooper, TrooperDescription desc)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var t = await _dbContext.FindAsync<Trooper>(trooper.Id);
            if (t is null)
                return new(false, new List<string> { "Trooper Not Found" });
            await _dbContext.Entry(t).Collection(e => e.Descriptions).LoadAsync();
            t.Descriptions.Sort((x, y) => x.Order.CompareTo(y.Order));

            t.Descriptions.RemoveAt(desc.Order);
            for (int i = 0; i < t.Descriptions.Count; i++)
            {
                t.Descriptions[i].Order = i;
            }
            var d = await _dbContext.FindAsync<TrooperDescription>(desc.Id);
            _dbContext.Remove(d);

            await _dbContext.SaveChangesAsync();
            return new(true, null);
        }

        public async Task<ResultBase> UpdateUserNameAsync(Trooper trooper)
        {
            var actual = await _userManager.FindByIdAsync(trooper.Id.ToString());
            var identResult = await _userManager.SetUserNameAsync(actual, trooper.UserName);

            if (!identResult.Succeeded)
            {
                List<string> errors = new();
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        public async Task<ResultBase> UpdateBirthNumberAsync(Trooper trooper)
        {
            var actual = await _userManager.FindByIdAsync(trooper.Id.ToString());
            actual.BirthNumber = trooper.BirthNumber;
            var identResult = await _userManager.UpdateAsync(actual);

            if (!identResult.Succeeded)
            {
                List<string> errors = new();
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        public async Task<ResultBase> DeleteAccountAsync(Trooper trooper, string password, ClaimsPrincipal claims)
        {
            var actual = await _userManager.FindByIdAsync(trooper.Id.ToString());
            var actualCurrent = await _userManager.GetUserAsync(claims);
            var validPassword = await _userManager.CheckPasswordAsync(actualCurrent, password);

            if (!validPassword)
            {
                return new(false, new() { "The provided password is invalid for your account." });
            }

            var identResult = await _userManager.DeleteAsync(actual);

            if (!identResult.Succeeded)
            {
                List<string> errors = new();
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        public async Task<ResultBase> UpdateAllowedNameChangersAsync(List<Trooper> allowedTroopers)
        {
            var oldSet = await GetAllowedNameChangersAsync();

            List<string> errors = new();
            foreach (var t in allowedTroopers)
            {
                if (!oldSet.Any(x => x.Id == t.Id))
                {
                    var actual = await _userManager.FindByIdAsync(t.Id.ToString());
                    var identResult = await _userManager.AddClaimAsync(actual, new("Change", "Name"));

                    if (!identResult.Succeeded)
                    {
                        foreach (var err in identResult.Errors)
                            errors.Add($"[{err.Code}] {err.Description}");
                    }
                }
            }

            foreach (var t in oldSet)
            {
                if (!allowedTroopers.Any(x => x.Id == t.Id))
                {
                    var actual = await _userManager.FindByIdAsync(t.Id.ToString());
                    var identResult = await _userManager.RemoveClaimAsync(actual, new("Change", "Name"));

                    if (!identResult.Succeeded)
                    {
                        foreach (var err in identResult.Errors)
                            errors.Add($"[{err.Code}] {err.Description}");
                    }
                }
            }

            if (errors.Count > 0)
                return new(false, errors);

            return new(true, null);
        }

        public async Task<ResultBase> UpdateNickNameAsync(Trooper trooper, int approver)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<Trooper>(trooper.Id);

            if (actual is null)
            {
                return new(false, new() { "The Trooper for that ID was not found." });
            }

            var old = actual.NickName;
            actual.NickName = trooper.NickName;

            var update = new NickNameUpdate()
            {
                ApprovedById = approver,
                ChangedOn = DateTime.UtcNow.ToEst(),
                OldNickname = old,
                NewNickname = actual.NickName,
            };

            actual.NickNameUpdates.Add(update);

            actual.LastUpdate = DateTime.UtcNow;

            try
            {
                _dbContext.Update(actual);
                await _dbContext.SaveChangesAsync();

                return new(true, null);
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message });
            }
        }

        public async Task<ResultBase> AddClaimAsync(Trooper trooper, Claim claim, int manager)
        {
            List<string> errors = new();
            var user = await _userManager.FindByIdAsync(trooper.Id.ToString());
            var identResult = await _userManager.AddClaimAsync(user, claim);

            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            user.ClaimUpdates.Add(new()
            {
                Additions = new() { new(claim.Type, claim.Value) },
                ChangedById = manager,
                ChangedOn = DateTime.UtcNow.ToEst()
            });

            identResult = await _userManager.UpdateAsync(user);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        public async Task<ResultBase> RemoveClaimAsync(Trooper trooper, Claim claim, int manager)
        {
            List<string> errors = new();
            var user = await _userManager.FindByIdAsync(trooper.Id.ToString());
            var identResult = await _userManager.RemoveClaimAsync(user, claim);

            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            user.ClaimUpdates.Add(new()
            {
                Removals = new() { new(claim.Type, claim.Value) },
                ChangedById = manager,
                ChangedOn = DateTime.UtcNow.ToEst()
            });

            identResult = await _userManager.UpdateAsync(user);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        public async Task<List<Claim>> GetAllClaimsFromTrooperAsync(Trooper trooper)
        {
            var user = await _userManager.FindByIdAsync(trooper.Id.ToString());
            return (await _userManager.GetClaimsAsync(user)).ToList();
        }

        public async Task<RegisterTrooperResult> ResetAccountAsync(Trooper trooper)
        {
            List<string> errors = new();
            var user = await _userManager.FindByIdAsync(trooper.Id.ToString());

            var identResult = await _userManager.RemovePasswordAsync(user);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, null, errors);
            }

            user.AccessCode = Guid.NewGuid().ToString();

            identResult = await _userManager.SetUserNameAsync(user, user.AccessCode);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, null, errors);
            }

            user.DiscordId = null;
            user.SteamLink = null;

            identResult = await _userManager.AddPasswordAsync(user, user.AccessCode);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, null, errors);
            }

            identResult = await _userManager.UpdateAsync(user);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, null, errors);
            }

            return new(true, user.AccessCode, null);
        }

        public async Task ValidateArchivedTroopersAsync()
        {
            HashSet<int> ids = new();
            using (var _dbContext = _dbContextFactory.CreateDbContext())
            {
                await _dbContext.Users
                    .AsNoTracking()
                    .Where(x => x.Slot == Slot.Archived)
                    .ForEachAsync(x => ids.Add(x.Id));
            }

            foreach (int id in ids)
            {
                var actual = await _userManager.FindByIdAsync(id.ToString());
                if (!await _userManager.IsInRoleAsync(actual, "Archived"))
                    await _userManager.AddToRoleAsync(actual, "Archived");
            }
        }
    }
}
