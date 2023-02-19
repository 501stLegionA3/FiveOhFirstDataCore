using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structuresbase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;

namespace FiveOhFirstDataCore.Api;

/// <summary>
/// Data Lookup API controller
/// </summary>
[Route("api")]
[ApiController]
public class LookupController : ControllerBase
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    /// <summary>
    /// Creates a new instance of the Lookup controller
    /// </summary>
    /// <param name="dbContextFactory">Db Context Factory</param>
    public LookupController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Full user data transfer object
    /// </summary>
    private class FullUserDTO
    {
        /// <summary>
        /// User's Nickname
        /// </summary>
        /// <example>Del</example>
        public string NickName { get; set; }

        /// <summary>
        /// User's Birth Number
        /// </summary>
        /// <example>11345</example>
        public int BirthNumber { get; set; }

        /// <summary>
        /// User's Last Promotion Date
        /// </summary>
        /// <example>2022-10-10T00:00:00</example>
        public DateTime LastPromotion { get; set; }

        /// <summary>
        /// User's Last Billet Change Date
        /// </summary>
        /// <example>2022-10-17T00:00:00</example>
        public DateTime LastBilletChange { get; set; }

        /// <summary>
        /// When the user joined
        /// </summary>
        /// <example>2022-10-24T18:57:06.704953</example>
        public DateTime StartOfService { get; set; }

        /// <summary>
        /// User's Slot
        /// </summary>
        /// <example>Avalanche 3-3</example>
        public string Slot { get; set; }

        /// <summary>
        /// User's Role
        /// </summary>
        /// <example>Squad Leader</example>
        public string Role { get; set; }

        /// <summary>
        /// User's Rank
        /// </summary>
        /// <example>Sergeant</example>
        public string Rank { get; set; }

        /// <summary>
        /// The user's qualifications as a csv
        /// </summary>
        /// <example>Zeus Permit, RTO Qualified, Marksman, Grenadier, Medic, Jumpmaster, Close Quarters Specalist</example>
        public string Qualifications { get; set; }

        /// <summary>
        /// The user's c-shops as a csv
        /// </summary>
        /// <example>Roster Staff, Holosite Support</example>
        public string CShops { get; set; }

        /// <summary>
        /// The user's TiG as number of days
        /// </summary>
        /// <example>15</example>
        public int TiG { get; set; }

        /// <summary>
        /// The user's TiB as number of days
        /// </summary>
        /// <example>8</example>
        public int TiB { get; set; }

        /// <summary>
        /// The user's TiS as number of days
        /// </summary>
        /// <example>0</example>
        public int TiS { get; set; }
    }

    /// <summary>
    /// A partial user class with only DisplayName and birth number
    /// </summary>
    private class UserDTO
    {
        /// <summary>
        /// User's Display name (Rank + Nickname)
        /// </summary>
        /// <example>CS Del</example>
        public string DisplayName { get; set; }

        /// <summary>
        /// User's Birth Number
        /// </summary>
        /// <example>11345</example>
        public int BirthNumber { get; set; }
    }

    /// <summary>
    /// Returns partial user data from birth number.
    /// </summary>
    /// <param name="user" example="11345">User's birth number</param>
    /// <param name="enableCsv" example="false">Output data as csv</param>
    /// <returns>An <see cref="ActionResult"/> for this request</returns>
    /// <response code="200">Returns a partial data class for the user.</response>
    /// <response code="404">The specified user could not be found.</response>
    [HttpGet("userinfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FullUserDTO))]
    [Produces("application/json", new[] { "text/plain" })]
    public async Task<ActionResult> UserInfo([FromQuery] int user, [FromQuery(Name = "csv")] bool enableCsv)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        //Find trooper
        var trooperQ = dbContext.Users.Where(e => e.BirthNumber == user);
        //Check if trooper is found
        if (await trooperQ.FirstOrDefaultAsync() == null)
            return NotFound(user.ToString());
        //Get qualification of trooper
        var qualificationsQ = Enum.GetValues<Qualification>().AsQueryable()
            .Where(x => x != Qualification.None && (trooperQ.First().Qualifications & x) == x).Select(x => x.AsFull());
        var qualifications = "";
        //If they have quals aggregate into single string
        if (qualificationsQ.Any())
            qualifications = qualificationsQ.Aggregate((a, b) => a + ", " + b);
        //Get c-shop positions of trooper
        var shopPositionsQ = Enum.GetValues<CShop>().AsQueryable()
            .Where(x => x != CShop.None && (trooperQ.First().CShops & x) == x).Select(x => x.AsFull());
        var shopPositions = "";
        //If they are in a c-shop aggregate into single string
        if (shopPositionsQ.Any())
            shopPositions = shopPositionsQ.Aggregate((a, b) => a + ", " + b);
        //Make trooper Data transfer object
        var trooper = trooperQ.Select(e =>
            new FullUserDTO()
            {
                NickName = e.NickName,
                BirthNumber = e.BirthNumber,
                LastPromotion = e.LastPromotion,
                LastBilletChange = e.LastBilletChange,
                StartOfService = e.StartOfService,
                Slot = e.Slot.AsFull(),
                Role = e.GetRoleName(),
                Rank = e.GetRankName(),
                Qualifications = qualifications,
                TiG = (DateTime.Now - e.LastPromotion).Days,
                TiB = (DateTime.Now - e.LastBilletChange).Days,
                TiS = (DateTime.Now - e.StartOfService).Days,
                CShops = shopPositions
            }
        ).First();
        //If not csv return json object
        if (!enableCsv)
            return Ok(trooper);
        //Make Csv string
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<FullUserDTO>();
        await csv.NextRecordAsync();
        csv.WriteRecord(trooper);
        await csv.FlushAsync();

        //Return Csv string
        return Ok(sw.ToString());
    }

    /// <summary>
    /// Returns partial user data for users that belong to the given slot/slots.
    /// </summary>
    /// <param name="slot" example="AvalancheThreeThree">Slot/slots to return.</param>
    /// <param name="role" example="Trooper">An optional parameter to filter by role.</param>
    /// <param name="enableCsv" example="false">Output data as csv.</param>
    /// <returns>An <see cref="ActionResult"/> for this request</returns>
    /// <response code="200">Returns a list of all users in the slot.</response>
    [HttpGet("billetinfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO[]))]
    [Produces("application/json", new[] { "text/plain" })]
    public async Task<ActionResult> BilletInfo([FromQuery(Name = "enum")] Slot[] slot, [FromQuery] Role? role,
        [FromQuery(Name = "csv")] bool enableCsv)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        //Get members who are in the slot
        var slotQuery = dbContext.Users.Where(t => slot.Contains(t.Slot));
        //If given a role filter by role
        if (role != null)
            //Get members who are in the slot and have the role
            slotQuery = slotQuery.Where(t => t.Role == role);
        //Create data transfer object
        var members = slotQuery.Select(t => new UserDTO()
            { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber });
        //If not csv return json object
        if (!enableCsv)
            return Ok(members.ToList());
        //Make Csv string
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<UserDTO>();
        await csv.NextRecordAsync();
        await csv.WriteRecordsAsync(members);
        await csv.FlushAsync();
        await sw.FlushAsync();

        //Return Csv string
        return Ok(sw.ToString());
    }

    /// <summary>
    /// Get all values from specified input;
    /// </summary>
    /// <param name="selectedEnum">Enum to lookup</param>
    /// <param name="enableCsv" example="false">Output data as csv.</param>
    /// <returns></returns>
    /// <response code="200">Returns all values in the enum.</response>
    /// <response code="404">The specified enum could not be found.</response>
    [HttpGet("enums")]
    public ActionResult Enums([FromQuery(Name = "enum")] string? selectedEnum,
        [FromQuery(Name = "csv")] bool enableCsv)
    {
        string tmp;
        var types = new[]
        {
            typeof(Slot), typeof(Role), typeof(CShop), typeof(Qualification), typeof(Team), typeof(Flight),
            typeof(PreferredRole)
        };
        var ranks = new[]
        {
            typeof(TrooperRank), typeof(WardenRank), typeof(PilotRank), typeof(RTORank), typeof(MedicRank),
            typeof(WarrantRank)
        };
        if (selectedEnum == null)
        {
            if (!enableCsv)
            {
                List<string> e = new();
                e.AddRange(types.Select(t => t.Name));
                e.Add("Rank");
                return Ok(e);
            }

            tmp = types.Select(t => t.Name).Aggregate("Enum,\n",(c, t) => c + t + ",\n") +"Rank";
            return Ok(tmp);
        }
        selectedEnum = selectedEnum.ToLower();
        
        if (selectedEnum == "rank")
        {
            if (!enableCsv)
            {
                Dictionary<string, List<string>> e = new();
                foreach (var rank in ranks)
                {
                    var name = rank.Name;
                    e.Add(name, new List<string>());
                    foreach (dynamic value in Enum.GetValues(rank))
                    {
                        e[name].Add(value.ToString());
                    }
                }

                return Ok(e);
            }

            tmp = ranks.Aggregate("Rank,\n",
                (current1, rank) => (current1 + Enum.GetValues(rank).Cast<dynamic>()
                    .Aggregate((current, enumValue) => (current + (enumValue.ToString() + ",\n")))));
        }
        else
        {
            var type = types.FirstOrDefault((a) => a.Name.ToLower() == selectedEnum);
            if (type == null)
                return BadRequest("Enum not found");
            if (!enableCsv)
            {
                return Ok(Enum.GetValues(type).Cast<dynamic>().Select(e => e.ToString()));
            }

            tmp = type.Name + ",\n" + Enum.GetValues(type).Cast<dynamic>().Aggregate((current, enumValue) =>
                (string)(current + (enumValue.ToString() + ",\n")));
        }

        return Ok(tmp);
    }

    /// <summary>
    /// Returns partial user data for users that belong to the given c-shop.
    /// </summary>
    /// <param name="cShopQuery" example="HolositeSupport">C-Shop to return.</param>
    /// <param name="enableCsv" example="false">Output data as csv.</param>
    /// <returns>An <see cref="ActionResult"/> for this request</returns>
    /// <response code="200">Returns a list of all users in the C-Shop.</response>
    [HttpGet("cshopinfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO[]))]
    [Produces("application/json", new[] { "text/plain" })]
    public async Task<ActionResult> CShopInfo([FromQuery(Name = "cshop")] CShop cShop,
        [FromQuery(Name = "csv")] bool enableCsv)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        //Get members who are in the c-shop
        var members = dbContext.Users.Where(t => (t.CShops & cShop) == cShop)
            //Create User data transfer object
            .Select(t => new UserDTO()
                { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber });
        //If not csv return json object
        if (!enableCsv)
            return Ok(members.ToList());
        //Make Csv string
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<UserDTO>();
        await csv.NextRecordAsync();
        await csv.WriteRecordsAsync(members);
        await csv.FlushAsync();
        await sw.FlushAsync();

        //Return Csv string
        return Ok(sw.ToString());
    }
}