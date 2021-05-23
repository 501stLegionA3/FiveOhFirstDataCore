using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Import;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    /// <summary>
    /// An implementation of <see cref="IImportService"/>, the <see cref="ImportService"/> holds methods to import 501st data that has been
    /// exported as CSVs.
    /// </summary>
    public class ImportService : IImportService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Trooper> _userManager;
        private readonly IWebHostEnvironment _env;

        public ImportService(ApplicationDbContext dbContext, UserManager<Trooper> userManager,
            IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _env = env;
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
                    var res = await ImportRosterAsync(import.RosterStream);
                    if (!res.GetResultWithWarnings(out var warn, out _))
                        return res;
                    else if (warn is not null)
                        warnings.AddRange(warn);
                }

                if (import.ZetaRosterStream is not null)
                {
                    toDelete.Add(import.ZetaRosterStream.Name);
                    var res = await ImportRosterAsync(import.ZetaRosterStream);
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
        /// Imports Active Roster data, creating and updating accountgs where needed.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> that contains Roster data.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        private async Task<ImportResult> ImportRosterAsync(FileStream stream)
        {
            // Birth number && name empty, take first item in that row - determines where troopers will go.

            // Convert CI, CM, etc. into respective MOS rank. Use Rank column for Trooper ranks.

            // Use Role column for the troopers role, where first team leader designates alpha and second
            // team leader designates bravo.

            // Other values are not needed to be edited, but *A* for acting needs to be pruned from nicnames.

            // Recruitment import will need to be a seprate import, as it requires all account data to populate the recruitment info.

            // MOS Rank = 0
            // Birth Number = 1
            // NickName = 2
            // Rank = 3,
            // Slot = 4 (and header, depending on position),
            // Role = 5,
            // Promotion = 8,
            // Start of Service = 9,
            // Inital = 11,
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
                    if (string.IsNullOrWhiteSpace(parts[1])
                        && string.IsNullOrWhiteSpace(parts[2]))
                    {
                        var p = parts[0];
                        if(group.Equals("Aviators"))
                        {
                            if(!int.TryParse(p[0..1], out flightNum))
                            {
                                flightNum = 0;
                                if(!p.Equals("HQ"))
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

                    var roleString = parts[5];
                    if(roleString.Equals("Squad Leader")
                        || roleString.Equals("Section Leader"))
                    {
                        currentTeam = null;
                    }

                    if(roleString.Equals("Team Leader"))
                    {
                        switch(currentTeam)
                        {
                            case null:
                                currentTeam = Team.Alpha;
                                break;
                            case Team.Alpha:
                                currentTeam = Team.Bravo;
                                break;
                        }
                    }

                    if (!int.TryParse(parts[1], out var id))
                    {
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
                    trooper.InitalTraining = parts[11];
                    trooper.UTC = parts[12];
                    trooper.Notes = string.Join(", ", parts[13..(parts.Length - 1)]);

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
                    if(setRole)
                    {
                        if (flightNum != 0)
                        {
                            if(roleString.Equals("Flight Commander"))
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
                            if(role is null)
                            {
                                role = roleString.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.GetRole();
                            }

                            if (role is not null)
                            {
                                trooper.Role = role.Value;
                                if(role == Role.Warden
                                    || role == Role.CheifWarden
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

                    if (!slotSet)
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
                        else if(group.Equals("HQ") && parts[4].Equals("Bastion Detachment"))
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
                    if(DateTime.TryParseExact(lastPromoString, "yyyyMMdd", customCulture, 
                        DateTimeStyles.AdjustToUniversal, out DateTime lastPromo))
                    {
                        trooper.LastPromotion = lastPromo;
                    }

                    var serviceStartString = parts[9];
                    if(DateTime.TryParseExact(serviceStartString, "yyyyMMdd", customCulture,
                        DateTimeStyles.AdjustToUniversal, out DateTime serviceStart))
                    {
                        trooper.StartOfService = serviceStart;
                    }

                    #region Grants
                    if(trooper.Rank >= TrooperRank.Corporal)
                    {
                        await _userManager.AddToRoleAsync(trooper, "NCO");
                    }

                    if(trooper.Role == Role.RTO)
                    {
                        await _userManager.AddToRoleAsync(trooper, "RTO");
                    }

                    if(trooper.Role == Role.Medic)
                    {
                        await _userManager.AddToRoleAsync(trooper, "Medic");
                    }

                    if(trooper.Role == Role.ARC)
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
                List<string> warnings = new();

                stream.Seek(0, SeekOrigin.Begin);
                using StreamReader sr = new(stream);

                Dictionary<int, List<string[]>> departments = new()
                {
                    { 1, new() },
                    { 3, new() },
                    { 4, new() },
                    { 5, new() },
                    { 6, new() },
                    { 7, new() },
                    { 8, new() },
                };

                string? line = null;
                int c = 0;
                while((line = await sr.ReadLineAsync()) is not null)
                {
                    // Line = Department
                    // 0-1 = C1
                    // 2-3 = C!
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
                    int key = 0;
                    for (int i = 0; i < parts.Length; i += 2)
                    {
                        // Get the Dict key for this section
                        switch(i)
                        {
                            case 0:
                            case 2:
                                key = 1;
                                break;
                            case 4:
                                // Nothing in this section.
                                continue;
                            case 6:
                                key = 3;
                                break;
                            case 8:
                                key = 4;
                                break;
                            case 10:
                                key = 5;
                                break;
                            case 12:
                            case 14:
                                key = 6;
                                break;
                            case 16:
                                key = 7;
                                break;
                            case 18:
                                key = 8;
                                break;
                        }

                        if (c == 2)
                        {
                            var smallerParts = parts[i].Split(':', StringSplitOptions.RemoveEmptyEntries);

                            departments[key].Add(smallerParts);
                        }
                        else
                        {
                            departments[key].Add(parts[i..(i+2)]);
                        }
                    }

                    // Simple Parse
                    // C1, C4, C5, C7, C8
                    // Not So Simple Parse
                    // C3, C6

                    List<Trooper> allTroopers = await _dbContext.Users
                        .ToListAsync();

                    foreach(var pair in departments)
                    {
                        var departmentList = pair.Value;
                        CShop? group = null;
                        foreach(var segment in departmentList)
                        {
                            try
                            {
                                if (string.IsNullOrWhiteSpace(segment[1]))
                                {
                                    group = segment[0].GetCShop();
                                    continue;
                                }

                                if(group is not null)
                                {
                                    var match = GetClosestTrooperMatch(segment[1], allTroopers);
                                    if (match is null)
                                    {
                                        warnings.Add($"Failed to find a trooper value for {segment[0]}");
                                        continue;
                                    }

                                    if (segment[0].Equals("Department Lead"))
                                    {
                                        await _userManager.AddClaimAsync(match, new("Department Lead", $"C{pair.Key}"));
                                    }
                                    else
                                    {
                                        if (group == CShop.CampaignManagement
                                            || group == CShop.QualTrainingStaff
                                            || group == CShop.MediaOutreach)
                                        {

                                        }
                                        else
                                        {
                                            string innerGroup = group.Value.AsFull();
                                            var innerTree = CShopExtensions.ClaimsTree[group.Value][innerGroup];
                                            var segmentParts = segment[0].Split(' ');

                                            string? claimValue;

                                            if(innerTree.Contains(segment[0]))
                                            {
                                                claimValue = segment[0];
                                            }   
                                            else if(innerTree.Contains(segmentParts[0]))
                                            {
                                                claimValue = segmentParts[0];
                                            }
                                            else if (innerTree.Contains(segmentParts.LastOrDefault() ?? ""))
                                            {
                                                claimValue = segmentParts.Last();
                                            }
                                            else
                                            {
                                                warnings.Add($"Failed to get a proper claim for {innerGroup} and {match.Id}");
                                                claimValue = null;
                                            }

                                            if(claimValue is not null)
                                            {
                                                await _userManager.AddClaimAsync(match, new(innerGroup, claimValue));
                                            }
                                        }
                                    }
                                }
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                warnings.Add($"Failed to properly parse a row of data: {string.Join(", ", segment)}");
                                continue;
                            }
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

        public Task<ImportResult> ImportSupportingElementsDataAsync(SupportingElementsImport import)
        {
            throw new NotImplementedException();
        }

        public void VerifyUnsafeFolder()
        {
            Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, _env.EnvironmentName, "unsafe_uploads"));
        }
    }
}
