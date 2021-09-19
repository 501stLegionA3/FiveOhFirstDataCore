using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Import;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Globalization;
using System.IO;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    /// <summary>
    /// An implementation of <see cref="IImportService"/>, the <see cref="ImportService"/> holds methods to import 501st data that has been
    /// exported as CSVs.
    /// </summary>
    public class ImportService : IImportService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<Trooper> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IWebsiteSettingsService _settings;

        private static readonly Dictionary<string, (string, string)> CShopClaimMatches = new()
        {
            { "committee lead", ("", "Lead") },
            { "section leader", ("", "Lead") },
            { "medals staff lead", ("", "Lead") },
            { "lead event manager", ("", "Lead") },
            { "teamspeak head", ("", "Lead") },
            { "lead admin", ("", "Lead") },
            { "commandant", ("", "Lead") },
            { "server lead", ("", "Lead") },
            { "editor in chief", ("", "Lead") },
            { "section assistant", ("", "Assistant") },
            { "assist. committee lead", ("", "Assistant") },
            { "medals staff sub lead", ("", "Assistant") },
            { "sub-lead event manager", ("", "Assistant") },
            { "teamspeak assistant", ("", "Assistant") },
            { "assistant commandant", ("", "Assistant") },
            { "server assistant", ("", "Assistant") },
            { "sr. roster clerk", ("", "Sr") },
            { "sr. recruiter", ("", "Sr") },
            { "sr. instructor", ("", "Sr") },
            { "senior instructor", ("", "Sr") },
            { "roster clerk", ("", "Clerk") },
            { "jr. roster clerk", ("", "Jr") },
            { "jr. instructor", ("", "Jr") },
            { "junior instructor", ("", "Jr") },
            { "lead curator", ("", "Curator") },
            { "section asst./game master", ("", "DeptAssistant") },
            { "head of documentation", ("", "Documentation") },
            { "event host", ("", "Host") },
            { "event helper", ("", "Helper") },
            { "permissions staff", ("", "Staff") },
            { "support", ("", "Staff") },
            { "admin", ("", "Staff") },
            { "spaceengineers", ("", "Space Engineers") },
            { "texture/helmet team lead", ("", "Texture Lead") },
            { "arma 3 units", ("", "Units") },
            { "tcw negotiator", ("", "TCW") },
            { "steam group rep", ("", "Steam") },
            { "moderator", ("", "Mod") },
            { "photo/graphics editor", ("", "Graphics Editor") },
            { "lead story writer", ("Story Writer", "Lead") },
            { "sub-lead story writer", ("Story Writer", "Assistant") },
            { "story writer", ("Story Writer", "Writer") },
            { "lead zeus operator", ("Zeus", "Lead") },
            { "sub-lead zeus operator", ("Zeus", "Assistant") },
            { "sr. zeus operator", ("Zeus", "Zeus") },
            { "lead mission builder", ("Mission Builder", "Lead") },
            { "sub-lead mission builder", ("Mission Builder", "Assistant") },
            { "sr. mission builder", ("Mission Builder", "Sr") },
            { "mission builder", ("Mission Builder", "Builder") },
            { "jr. mission builder", ("Mission Builder", "Jr") },
            { "campaign artisan lead", ("Artisan", "Lead") },
            { "campaign artisan sub-lead", ("Artisan", "Assistant") },
            { "campaign artisan", ("Artisan", "Artisan") },
            { "logistics", ("Logistics", "Staff") },
            { "lead rto mos instructor", ("RTO", "Lead") },
            { "rto mos instructor", ("RTO", "Instructor") },
            { "lead medical mos instructor", ("Medical", "Lead") },
            { "medical mos instructor", ("Medical", "Instructor") },
            { "medical mos cadre", ("Medical", "Cadre") },
            { "lead assault instructor", ("Assault", "Lead") },
            { "assault instructor", ("Assault", "Instructor") },
            { "lead support instructor", ("Support", "Lead") },
            { "support instructor", ("Support", "Instructor") },
            { "lead marksman instructor", ("Marksman", "Lead") },
            { "marksman instructor", ("Marksman", "Instructor") },
            { "lead grenadier instructor", ("Grenadier", "Lead") },
            { "grenadier instructor", ("Grenadier", "Instructor") },
            { "lead jump-master instructor", ("Jump-Master", "Lead") },
            { "jump-master instructor", ("Jump-Master", "Instructor") },
            { "lead combat engineer instructor", ("Combat Engineer", "Lead") },
            { "combat engineer instructor", ("Combat Engineer", "Instructor") },
            { "youtube lead", ("YouTube", "Lead") },
            { "youtube assistant", ("YouTube", "Lead") },
            { "youtube staff", ("YouTube", "Staff") },
            { "facebook manager", ("Facebook", "Staff") },
            { "reddit manager", ("Reddit", "Staff") },
            { "twitter manager", ("Twitter", "Staff") },
            { "tiktok lead", ("TikTok", "Lead") },
            { "tiktok staff", ("TikTok", "Staff") },
            { "chief instructor", ("", "Chief") },
            { "medals staff", ("", "Staff") }
        };

        private static readonly Dictionary<int, Qualification> RowQualBindings = new()
        {
            { 2, Qualification.Assault },
            { 3, Qualification.RAMR },
            { 4, Qualification.Support },
            { 5, Qualification.Z1000 },
            { 6, Qualification.Grenadier },
            { 7, Qualification.Marksman },
            { 8, Qualification.Jumpmaster },
            { 9, Qualification.CombatEngineer },
            { 10, Qualification.RTOBasic },
            { 11, Qualification.RTOQualified }
        };

        private Dictionary<CShop, CShopClaim>? ClaimsTree { get; set; } = null;

        public ImportService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<Trooper> userManager,
            IWebHostEnvironment env, IWebsiteSettingsService settings)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _env = env;
            _settings = settings;
        }

        public async Task<ImportResult> ImportOrbatDataAsync(OrbatImport import)
        {
            List<string> toDelete = new();
            try
            {
                List<string> warnings = new();
                if (import.RosterStream is not null)
                {
                    toDelete.Add(import.RosterStream.Name);
                    var res = await ImportRosterAsync(import.RosterStream, (false, false));
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                if (import.ZetaRosterStream is not null)
                {
                    toDelete.Add(import.ZetaRosterStream.Name);
                    var res = await ImportRosterAsync(import.ZetaRosterStream, (false, false));
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                if (import.CShopStream is not null)
                {
                    toDelete.Add(import.CShopStream.Name);
                    var res = await ImportCShopAsync(import.CShopStream);
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                return new(true, null, warnings);
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message }, new());
            }
            finally
            {
                await import.DisposeAsync();
                foreach (var s in toDelete)
                {
                    try
                    {
                        File.Delete(s);
                    }
                    catch { continue; }
                }
            }
        }

        /// <summary>
        /// Imports Active Roster data, creating and updating accounts where needed.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> that contains Roster data.</param>
        /// <param name="supportingElements">Two boolean values determining if this import is for the supporting elements rosters. Item1 
        /// represents if this is for supporting elements. Item2 to determines if this is specifcally for the training form.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        private async Task<ImportResult> ImportRosterAsync(FileStream stream, (bool, bool) supportingElements)
        {
            // Birth number && name empty, take first item in that row - determines where troopers will go.

            // Convert CI, CM, etc. into respective MOS rank. Use Rank column for Trooper ranks.

            // Use Role column for the troopers role, where first team leader designates alpha and second
            // team leader designates bravo.

            // Other values are not needed to be edited, but *A* for acting needs to be pruned from nicknames.

            // Recruitment import will need to be a seprate import, as it requires all account data to populate the recruitment info.

            // MOS Rank = 0
            // Birth Number = 1
            // NickName = 2
            // Rank = 3,
            // Slot = 4 (and header, depending on position),
            // Role = 5,
            // Promotion = 8,
            // Start of Service = 9,
            // Initial = 11,
            // UTC = 12,
            // Notes = All remaining rows.
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using StreamReader sr = new(stream);

                string? line = "";
                string group = "";
                int c = 0;
                List<string> warnings = new();
                // Start this at the back, the first run returns it to no team.
                Team? currentTeam = Team.Bravo;
                // Start this at the back, the first run will reset it to Alpha.
                Flight flight = Flight.Delta;
                int flightNum = 0;
                int wingNum = 0;
                while ((line = await sr.ReadLineAsync()) is not null)
                {
                    // Skip the header rows.
                    if (c++ < 2) continue;

                    var parts = line.Split(',', StringSplitOptions.TrimEntries);

                    // Check the Birth # and Nickname fields. If both are empty,
                    // set the group header.

                    // If its supporting elements, ignore this.
                    var roleString = parts[5];
                    if (!supportingElements.Item1)
                    {
                        if (string.IsNullOrWhiteSpace(parts[1])
                            && string.IsNullOrWhiteSpace(parts[2]))
                        {
                            var p = parts[0];
                            if (group.Equals("Aviators"))
                            {
                                if (!int.TryParse(p[0..1], out flightNum))
                                {
                                    flightNum = 0;
                                    if (!p.Equals("HQ"))
                                        group = p;
                                }
                            }
                            else
                            {
                                group = p;
                            }
                            continue;
                        }

                        if (parts[5].Equals("Pilot"))
                        {
                            switch (flight)
                            {
                                case Flight.Alpha:
                                    flight = Flight.Bravo;
                                    break;
                                case Flight.Bravo:
                                    flight = Flight.Charlie;
                                    break;
                                case Flight.Charlie:
                                    flight = Flight.Delta;
                                    break;
                                case Flight.Delta:
                                    flight = Flight.Alpha;
                                    wingNum++;
                                    if (wingNum > 2) wingNum = 1;
                                    break;
                            }
                        }

                        if (roleString.Equals("Squad Leader")
                            || roleString.Equals("Section Leader"))
                        {
                            currentTeam = null;
                        }

                        if (roleString.Equals("Team Leader"))
                        {
                            switch (currentTeam)
                            {
                                case null:
                                    currentTeam = Team.Alpha;
                                    break;
                                case Team.Alpha:
                                    currentTeam = Team.Bravo;
                                    break;
                            }
                        }
                    }

                    if (!int.TryParse(parts[1], out var id))
                    {
                        if (!string.IsNullOrWhiteSpace(parts[1]))
                            warnings.Add($"{parts[1]} was unable to be parsed as an ID");
                        continue;
                    }

                    var trooper = await _userManager.FindByIdAsync(parts[1]);

                    if (trooper is null)
                    {
                        string code = Guid.NewGuid().ToString();
                        trooper = new Trooper()
                        {
                            Id = id,
                            AccessCode = code,
                            UserName = code
                        };

                        var identRes = await _userManager.CreateAsync(trooper, code);

                        if (!identRes.Succeeded)
                        {
                            foreach (var error in identRes.Errors)
                                warnings.Add($"Failed to create a user for {id}: [{error.Code}] {error.Description}");

                            continue;
                        }
                    }

                    trooper.NickName = parts[2].Replace("*A*", string.Empty).Trim();
                    if (!supportingElements.Item2)
                    {
                        trooper.InitialTraining = parts[11];
                        trooper.UTC = parts[12];
                        trooper.Notes = string.Join(", ", parts[13..(parts.Length - 1)]);
                    }
                    else
                    {
                        trooper.Notes = string.Join(", ", parts[11..(parts.Length - 1)]);
                    }

                    Enum? rank;
                    bool setRank = true;
                    bool setRole = true;
                    if ((rank = parts[0].GetRank()) is not null)
                    {
                        switch (rank)
                        {
                            case TrooperRank r:
                                trooper.Rank = r;
                                trooper.Team = currentTeam;
                                break;
                            case MedicRank m:
                                trooper.MedicRank = m;
                                trooper.Role = Role.Medic;
                                trooper.Team = currentTeam;
                                setRole = false;
                                break;
                            case RTORank r:
                                trooper.RTORank = r;
                                trooper.Role = Role.RTO;
                                setRole = false;
                                break;
                            case PilotRank p:
                                trooper.PilotRank = p;
                                setRank = false;
                                break;
                            case WardenRank w:
                                if ((rank = parts[3].GetRank()) is not null)
                                    trooper.WardenRank = rank as WardenRank?;
                                setRank = false;
                                break;
                            case WarrantRank w:
                                trooper.WarrantRank = w;
                                trooper.Team = currentTeam;
                                setRank = false;
                                break;
                        }
                    }

                    if (setRank)
                    {
                        rank = parts[3].GetRank();
                        if (rank is TrooperRank t)
                        {
                            trooper.Rank = t;
                        }
                        else
                        {
                            warnings.Add($"Was unable to parse a trooper rank for {id}");
                        }
                    }

                    bool slotSet = false;
                    if (supportingElements.Item2)
                    {
                        trooper.Role = Role.Trooper;
                    }
                    else if (setRole)
                    {
                        if (flightNum != 0)
                        {
                            if (roleString.Equals("Flight Commander"))
                            {
                                trooper.Role = Role.Commander;
                                trooper.Slot = flightNum.GetRazorSlot(0);
                            }
                            else
                            {
                                trooper.Flight = flight;
                                trooper.Role = Role.Pilot;
                                trooper.Slot = flightNum.GetRazorSlot(wingNum);
                                if (wingNum == 1)
                                    trooper.Team = Team.Alpha;
                                else trooper.Team = Team.Bravo;
                            }

                            slotSet = true;
                        }
                        else if (roleString.Equals("Squadron Commander"))
                        {
                            trooper.Slot = Slot.Razor;
                            trooper.Role = Role.Commander;
                            slotSet = true;
                        }
                        else if (roleString.Equals("Squadron Sub-Commander"))
                        {
                            trooper.Slot = Slot.Razor;
                            trooper.Role = Role.SubCommander;
                            slotSet = true;
                        }
                        else if (roleString.Equals("Airborne Sergeant"))
                        {
                            trooper.Role = Role.SergeantMajor;
                        }
                        else if (roleString.Equals("Battalion SC"))
                        {
                            trooper.Role = Role.CShopCommander;
                        }
                        else if (roleString.Equals("Shop Commander XO"))
                        {
                            trooper.Role = Role.CShopXO;
                        }
                        else
                        {
                            Role? role = roleString.GetRole();
                            if (role is null)
                            {
                                role = roleString.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.GetRole();
                            }

                            if (role is not null)
                            {
                                trooper.Role = role.Value;
                                if (role == Role.Warden
                                    || role == Role.ChiefWarden
                                    || role == Role.MasterWarden)
                                {
                                    trooper.Slot = Slot.Warden;
                                    slotSet = true;
                                }
                                else if (role == Role.ARC)
                                {
                                    trooper.Team = null;
                                }
                            }
                            else
                            {
                                warnings.Add($"Failed to find a role for {id}");
                            }
                        }
                    }

                    if (supportingElements.Item1)
                    {
                        if (parts[4].StartsWith("Mynock"))
                            trooper.Slot = Slot.MynockReserve;
                        else if (parts[4].StartsWith("Acklay"))
                            trooper.Slot = Slot.AcklayReserve;
                        else if (parts[4].StartsWith(("Razor")))
                            trooper.Slot = Slot.RazorReserve;
                        else trooper.Slot = Slot.InactiveReserve;
                    }
                    else if (!slotSet)
                    {
                        var slot = parts[4].Replace("st", string.Empty)
                            .Replace("nd", string.Empty)
                            .Replace("rd", string.Empty)
                            .GetSlot();
                        if (slot is not null)
                        {
                            trooper.Slot = slot.Value;
                        }
                        else if ((slot = group.GetSlot()) is not null)
                        {
                            trooper.Slot = slot.Value;
                        }
                        else if (group.Equals("HQ") && parts[4].Equals("Bastion Detachment"))
                        {
                            trooper.Slot = Slot.Mynock;
                        }
                        else
                        {
                            warnings.Add($"Unable to parse a Slot for {id}");
                        }
                    }

                    var customCulture = new CultureInfo("en-US", true);
                    customCulture.DateTimeFormat.ShortDatePattern = "yyyyMMdd";

                    // last promotion = 8
                    // start of service = 9
                    var lastPromoString = parts[8];
                    if (DateTime.TryParseExact(lastPromoString, "yyyyMMdd", customCulture,
                        DateTimeStyles.AdjustToUniversal, out DateTime lastPromo))
                    {
                        trooper.LastPromotion = lastPromo;
                    }

                    var serviceStartString = parts[9];
                    if (DateTime.TryParseExact(serviceStartString, "yyyyMMdd", customCulture,
                        DateTimeStyles.AdjustToUniversal, out DateTime serviceStart))
                    {
                        trooper.StartOfService = serviceStart;
                    }

                    #region Grants
                    if (trooper.Rank >= TrooperRank.Corporal)
                    {
                        await _userManager.AddToRoleAsync(trooper, "NCO");
                    }

                    if (trooper.Role == Role.RTO)
                    {
                        await _userManager.AddToRoleAsync(trooper, "RTO");
                    }

                    if (trooper.Role == Role.Medic)
                    {
                        await _userManager.AddToRoleAsync(trooper, "Medic");
                    }

                    if (trooper.Role == Role.ARC)
                    {
                        await _userManager.AddToRoleAsync(trooper, "ARC");
                    }

                    await _userManager.AddToRoleAsync(trooper, "Trooper");
                    #endregion

                    await _userManager.UpdateAsync(trooper);
                }

                return new(true, null, warnings);
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message }, new());
            }
        }

        /// <summary>
        /// Imports the CShop roster data, modifying accounts with the proper claims.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> that contains C-Shop data.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        private async Task<ImportResult> ImportCShopAsync(FileStream stream)
        {
            try
            {
                using var _dbContext = _dbContextFactory.CreateDbContext();

                List<string> warnings = new();

                stream.Seek(0, SeekOrigin.Begin);
                using StreamReader sr = new(stream);

                Dictionary<int, List<string[]>> departments = new()
                {
                    { 0, new() },
                    { 2, new() },
                    { 6, new() },
                    { 8, new() },
                    { 10, new() },
                    { 12, new() },
                    { 14, new() },
                    { 16, new() },
                    { 18, new() },
                };

                string? line = null;
                int c = 0;
                while ((line = await sr.ReadLineAsync()) is not null)
                {
                    // Line = Department
                    // 0-1 = C1
                    // 2-3 = C1
                    // 4-5 = N/A
                    // 6-7 = C3
                    // 8-9 = C4
                    // 10-11 = C5
                    // 12-13 = C6
                    // 14-15 = C6
                    // 16-17 = C7
                    // 18-19 = C8

                    // Skip the header row.
                    if (c++ < 1) continue;

                    var parts = line.Split(',');
                    for (int i = 0; i < parts.Length; i += 2)
                    {
                        if (i == 4) continue;

                        if (!string.IsNullOrWhiteSpace(parts[i]))
                        {
                            if (c == 2)
                            {
                                var smallerParts = parts[i].Split(':', StringSplitOptions.RemoveEmptyEntries);

                                departments[i].Add(smallerParts);
                            }
                            else
                            {
                                departments[i].Add(parts[i..(i + 2)]);
                            }
                        }
                    }
                }

                // Simple Parse
                // C1, C4, C5, C7, C8
                // Not So Simple Parse
                // C3, C6

                List<Trooper> allTroopers = await _dbContext.Users
                    .ToListAsync();

                foreach (var pair in departments)
                {
                    var departmentList = pair.Value;
                    CShop? group = null;
                    foreach (var segment in departmentList)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(segment[1]))
                            {
                                // Line = Department
                                // 0-1 = C1
                                // 2-3 = C1
                                // 4-5 = N/A
                                // 6-7 = C3
                                // 8-9 = C4
                                // 10-11 = C5
                                // 12-13 = C6
                                // 14-15 = C6
                                // 16-17 = C7
                                // 18-19 = C8

                                group = segment[0].GetCShop();
                                continue;
                            }

                            if (group is not null)
                            {
                                if (segment[1].Equals("TBD", StringComparison.OrdinalIgnoreCase)
                                    || segment[1].Equals("Closed", StringComparison.OrdinalIgnoreCase))
                                    continue;

                                var match = GetClosestTrooperMatch(segment[1], allTroopers);
                                if (match is null)
                                {
                                    warnings.Add($"Failed to find a trooper value for {segment[1]} in {segment[0]}");
                                    continue;
                                }

                                var edited = segment[0];

                                Claim? claim;
                                if (edited.Equals("department lead"))
                                {
                                    // 0-1 = C1
                                    // 2-3 = C1
                                    // 4-5 = N/A
                                    // 6-7 = C3
                                    // 8-9 = C4
                                    // 10-11 = C5
                                    // 12-13 = C6
                                    // 14-15 = C6
                                    // 16-17 = C7
                                    // 18-19 = C8
                                    int key = pair.Key switch
                                    {
                                        6 => 3,
                                        8 => 4,
                                        10 => 5,
                                        12 => 6,
                                        14 => 6,
                                        16 => 7,
                                        18 => 8,
                                        _ => 1,
                                    };

                                    claim = new("Department Lead", $"C{key}");
                                }
                                else
                                {
                                    claim = await GetclaimForCShop(segment[0].Trim().ToLower(), group.Value);
                                }

                                if (claim is not null)
                                {
                                    match.CShops |= group.Value;
                                    await _userManager.RemoveClaimAsync(match, claim);
                                    await _userManager.AddClaimAsync(match, claim);
                                    await _dbContext.SaveChangesAsync();
                                }
                                else
                                    warnings.Add($"Unabled to assign claim for {match.Id} in {group.Value.AsFull()} for {segment[0]}");
                            }
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            warnings.Add($"Failed to properly parse a row of data: {string.Join(", ", segment)}\nwith error: {ex}");
                            continue;
                        }

                    }
                }

                return new(true, null, warnings);
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message }, new());
            }
        }

        private static Trooper? GetClosestTrooperMatch(string trooperName, List<Trooper> troopers)
        {
            var possible = troopers.Where(x => x.NickName.Equals(trooperName.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            if (possible.Count > 1)
                possible.Sort((x, y) => x.Slot.CompareTo(y.Slot));
            return possible.FirstOrDefault();
        }

        private async Task<Claim?> GetclaimForCShop(string role, CShop group)
        {
            if (CShopClaimMatches.TryGetValue(role, out var pair))
            {
                if (string.IsNullOrWhiteSpace(pair.Item1)) return new(group.AsFull(), pair.Item2);
                else return new(pair.Item1, pair.Item2);
            }

            if (ClaimsTree is null)
                ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            // Nothing custom matched, so lets look directly.
            if (ClaimsTree.TryGetValue(group, out var item))
                foreach (var sets in item.ClaimData)
                    foreach (var value in sets.Value)
                        if (value.Equals(role, StringComparison.OrdinalIgnoreCase))
                            return new(sets.Key, role);

            return null;
        }

        public async Task<ImportResult> ImportSupportingElementsDataAsync(SupportingElementsImport import)
        {
            List<string> toDelete = new();
            try
            {
                List<string> warnings = new();
                if (import.InactiveReservesStream is not null)
                {
                    toDelete.Add(import.InactiveReservesStream.Name);
                    var res = await ImportRosterAsync(import.InactiveReservesStream, (true, false));
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                if (import.TrainingStream is not null)
                {
                    toDelete.Add(import.TrainingStream.Name);
                    var res = await ImportRosterAsync(import.TrainingStream, (true, true));
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                return new(true, null, warnings);
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message }, new());
            }
            finally
            {
                await import.DisposeAsync();
                foreach (var s in toDelete)
                {
                    try
                    {
                        File.Delete(s);
                    }
                    catch { continue; }
                }
            }
        }

        public async Task<ImportResult> ImportQualificationDataAsync(QualificationImport import)
        {
            List<string> toDelete = new();
            try
            {
                List<string> warnings = new();
                if (import.UnifiedQualStream is not null)
                {
                    toDelete.Add(import.UnifiedQualStream.Name);
                    var res = await ImportQualificationUnificationSheetAsync(import.UnifiedQualStream);
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                return new(true, null, warnings);
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message }, new());
            }
            finally
            {
                await import.DisposeAsync();
                foreach (var s in toDelete)
                {
                    try
                    {
                        File.Delete(s);
                    }
                    catch { continue; }
                }
            }
        }

        private async Task<ImportResult> ImportQualificationUnificationSheetAsync(FileStream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using StreamReader sr = new(stream);

                string? line = "";
                List<string> warnings = new();
                while ((line = await sr.ReadLineAsync()) is not null)
                {
                    var parts = line.Split(',', StringSplitOptions.TrimEntries);

                    if (!int.TryParse(parts[1], out var id))
                    {
                        if (!string.IsNullOrWhiteSpace(parts[1]))
                            warnings.Add($"{parts[1]} was unable to be parsed as an ID");
                        continue;
                    }

                    var trooper = await _userManager.FindByIdAsync(parts[1]);

                    if (trooper is null)
                    {
                        warnings.Add($"Unable to find a trooper by the ID of {parts[1]}");
                        continue;
                    }

                    for (int i = 2; i < parts.Length; i++)
                    {
                        if (parts[i].Equals("pass", StringComparison.OrdinalIgnoreCase))
                            if (RowQualBindings.TryGetValue(i, out var qual))
                                trooper.Qualifications |= qual;
                    }
                }

                return new(true, warnings, new());
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message }, new());
            }
        }

        public void VerifyUnsafeFolder()
        {
            Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, _env.EnvironmentName, "unsafe_uploads"));
        }
    }
}

// Was used as a basis for the first part of geting claims in CShops
//#region Claim Switch
//switch (segment[0].Trim().ToLower())
//{
//    case "department lead":
//        claim = new("Department Lead", $"C{pair.Key}");
//        break;
//    case "committee lead":
//    case "section leader":
//    case "medals staff lead":
//    case "lead event manager":
//    case "teamspeak head":
//    case "lead admin":
//    case "commandant":
//    case "server lead":
//    case "editor in chief":
//        claim = new(innerTitleBase, "Lead");
//        break;
//    case "section assistant":
//    case "assit. committee lead":
//    case "medals staff sub lead":
//    case "sub-lead event manager":
//    case "teamspeak assistant":
//    case "assistant commandant":
//    case "server assistant":
//        claim = new(innerTitleBase, "Assistant");
//        break;
//    case "sr. roster clerk":
//    case "sr. recruiter":
//    case "sr. instructor":
//    case "senior instructor":
//        claim = new(innerTitleBase, "Sr");
//        break;

//    case "roster clerk":
//        claim = new(innerTitleBase, "Clerk");
//        break;
//    case "jr. roster clerk":
//    case "jr. instructor":
//    case "junior instructor":
//        claim = new(innerTitleBase, "Jr");
//        break;

//    case "lead curator":
//        claim = new(innerTitleBase, "Curator");
//        break;
//    case "section asst./game master":
//        claim = new(innerTitleBase, "DeptAssistant");
//        break;

//    case "head of documentation":
//        claim = new(innerTitleBase, "Documentation");
//        break;
//    case "event host":
//        claim = new(innerTitleBase, "Host");
//        break;
//    case "event helper":
//        claim = new(innerTitleBase, "Helper");
//        break;

//    case "permissions staff":
//    case "support":
//    case "admin":
//        claim = new(innerTitleBase, "Staff");
//        break;

//    case "spaceengineers":
//        claim = new(innerTitleBase, "Space Engineers");
//        break;

//    case "texture/helmet team lead":
//        claim = new(innerTitleBase, "Texture Lead");
//        break;

//    case "arma 3 units":
//        claim = new(innerTitleBase, "Units");
//        break;
//    case "tcw negotiator":
//        claim = new(innerTitleBase, "TCW");
//        break;
//    case "steam group rep":
//        claim = new(innerTitleBase, "Steam");
//        break;

//    case "moderator":
//        claim = new(innerTitleBase, "Mod");
//        break;

//    case "photo/graphics editor":
//        claim = new(innerTitleBase, "Graphics Editor");
//        break;

//    #region Campaign Management
//    case "lead story writer":
//        claim = new("Story Writer", "Lead");
//        break;
//    case "sub-lead story writer":
//        claim = new("Story Writer", "Assistant");
//        break;
//    case "story writer":
//        claim = new("Story Writer", "Writer");
//        break;

//    case "lead zeus operator":
//        claim = new("Zeus", "Lead");
//        break;
//    case "sub-lead zeus operator":
//        claim = new("Zeus", "Assistant");
//        break;
//    case "sr. zeus operator":
//        claim = new("Zeus", "Zeus");
//        break;

//    case "lead mission builder":
//        claim = new("Mission Builder", "Lead");
//        break;
//    case "sub-lead mission builder":
//        claim = new("Mission Builder", "Assistant");
//        break;
//    case "sr. mission builder":
//        claim = new("Mission Builder", "Sr");
//        break;
//    case "mission builder":
//        claim = new("Mission Builder", "Builder");
//        break;
//    case "jr. mission builder":
//        claim = new("Mission Builder", "Jr");
//        break;

//    case "campaign artisan lead":
//        claim = new("Artisan", "Lead");
//        break;
//    case "campaign artisan sub-lead":
//        claim = new("Artisan", "Assistant");
//        break;
//    case "campaign artisan":
//        claim = new("Artisan", "Artisan");
//        break;

//    case "logistics":
//        claim = new("Logistics", "Staff");
//        break;
//    #endregion

//    #region Qualification/MOS Training Staff
//    case "lead rto mos instructor":
//        claim = new("RTO", "Lead");
//        break;
//    case "rto mos instructor":
//        claim = new("RTO", "Instructor");
//        break;

//    case "lead medical mos instructor":
//        claim = new("Medical", "Lead");
//        break;
//    case "medical mos instructor":
//        claim = new("Medical", "Instructor");
//        break;
//    case "medical mos cadre":
//        claim = new("Medical", "Cadre");
//        break;

//    case "lead assault mos instructor":
//        claim = new("Assault", "Lead");
//        break;
//    case "assault mos instructor":
//        claim = new("Assault", "Instructor");
//        break;

//    case "lead support mos instructor":
//        claim = new("Support", "Lead");
//        break;
//    case "support mos instructor":
//        claim = new("Support", "Instructor");
//        break;

//    case "lead marksman mos instructor":
//        claim = new("Marksman", "Lead");
//        break;
//    case "marksman mos instructor":
//        claim = new("Marksman", "Instructor");
//        break;

//    case "lead grenadier mos instructor":
//        claim = new("Grenadier", "Lead");
//        break;
//    case "grenadier mos instructor":
//        claim = new("Grenadier", "Instructor");
//        break;

//    case "lead jump-master mos instructor":
//        claim = new("Jump-Master", "Lead");
//        break;
//    case "jump-master mos instructor":
//        claim = new("Jump-Master", "Instructor");
//        break;

//    case "lead combat engineer mos instructor":
//        claim = new("Combat Engineer", "Lead");
//        break;
//    case "combat engineer mos instructor":
//        claim = new("Combat Engineer", "Instructor");
//        break;
//    #endregion

//    #region Media Outreach
//    case "youtube lead":
//        claim = new("YouTube", "Lead");
//        break;
//    case "youtube assistant":
//        claim = new("YouTube", "Lead");
//        break;
//    case "youtube staff":
//        claim = new("YouTube", "Staff");
//        break;

//    case "facebook manager":
//        claim = new("Facebook", "Staff");
//        break;
//    case "reddit manager":
//        claim = new("Reddit", "Staff");
//        break;
//    case "twitter manager":
//        claim = new("Twitter", "Staff");
//        break;

//    case "tiktok lead":
//        claim = new("TikTok", "Lead");
//        break;
//    case "tiktok staff":
//        claim = new("TikTok", "Staff");
//        break;
//    #endregion

//    default:
//        foreach (var item in CShopExtensions.ClaimsTree)
//            foreach (var sets in item.Value)
//                foreach (var value in sets.Value)
//                    if (value.Equals(segment[0], StringComparison.OrdinalIgnoreCase)
//                        && group.Value == item.Key)
//                        claim = new(sets.Key, segment[0]);

//        if (claim is null)
//        {
//            warnings.Add($"Failed to find a claim for {match.Id} in {innerTitleBase}");
//            continue;
//        }
//        break;
//}
//#endregion
