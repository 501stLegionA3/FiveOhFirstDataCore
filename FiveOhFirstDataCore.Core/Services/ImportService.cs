using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Import;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Esf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public ImportService(ApplicationDbContext dbContext, UserManager<Trooper> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ImportResult> ImportOrbatDataAsync(OrbatImport import)
        {
            try
            {
                if (import.RosterStream is not null)
                {
                    var res = await ImportRosterAsync(import.RosterStream);
                    if (!res.GetResult(out _))
                        return res;
                }

                if (import.ZetaRosterStream is not null)
                {
                    var res = await ImportRosterAsync(import.ZetaRosterStream);
                    if (!res.GetResult(out _))
                        return res;
                }

                if (import.CShopStream is not null)
                {
                    var res = await ImportRosterAsync(import.CShopStream);
                    if (!res.GetResult(out _))
                        return res;
                }

                return new(true, null);
            }
            finally
            {
                await import.DisposeAsync();
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

            using StreamReader sr = new(stream);

            string? line = "";
            string group = "";
            int c = 0;
            List<string> warnings = new();
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
                    group = parts[0];
                    continue;
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
                if ((rank = parts[0].GetRank()) is not null)
                {
                    switch (rank)
                    {
                        case TrooperRank r:
                            trooper.Rank = r;
                            setRank = false;
                            break;
                        case MedicRank m:
                            trooper.MedicRank = m;
                            break;
                        case RTORank r:
                            trooper.RTORank = r;
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
                            break;
                    }
                }

                if (setRank)
                {
                    rank = parts[2].GetRank();
                    if (rank is TrooperRank t)
                    {
                        trooper.Rank = t;
                    }
                    else
                    {
                        warnings.Add($"Was unable to parse a trooper rank for {id}");
                    }
                }


            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Imports the Zeta Roster data, creating and updating accounts where needed.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> that contains Zeta Roster data.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        private async Task<ImportResult> ImportZetaRosterAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Imports the CShop roster data, modifying accounts with the proper claims.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> that contains C-Shop data.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        private async Task<ImportResult> ImportCShopAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }

        public Task<ImportResult> ImportSupportingElementsDataAsync(SupportingElementsImport import)
        {
            throw new NotImplementedException();
        }
    }
}
