using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structuresbase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using CsvHelper;
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
        _dbContextFactory = dbContextFactory; }

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
        var trooperQ = dbContext.Users.Where(e => e.BirthNumber == user);
        if (await trooperQ.FirstOrDefaultAsync() == null)
            return NotFound(user.ToString());
        var qualificationsQ = Enum.GetValues<Qualification>().AsQueryable()
            .Where(x => x != Qualification.None && (trooperQ.First().Qualifications & x) == x).Select(x => x.AsFull());
        var qualifications = "";
        if (qualificationsQ.Any())
            qualifications = qualificationsQ.Aggregate((a, b) => a + ", " + b);
        var shopPositionsQ = Enum.GetValues<CShop>().AsQueryable()
            .Where(x => x != CShop.None && (trooperQ.First().CShops & x) == x).Select(x => x.AsFull());
        var shopPositions = "";
        if (shopPositionsQ.Any())
            shopPositions = shopPositionsQ.Aggregate((a, b) => a + ", " + b);
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
        if (!enableCsv)
            return Ok(trooper);
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<FullUserDTO>();
        await csv.NextRecordAsync();
        csv.WriteRecord(trooper);
        await csv.FlushAsync();

        return Ok(sw.ToString());
    }

    /// <summary>
    /// Returns partial user data for users that belong to the given slot.
    /// </summary>
    /// <param name="slot" example="AvalancheThreeThree">Slot to return.</param>
    /// <param name="role" example="Trooper">An optional parameter to filter by role.</param>
    /// <param name="enableCsv" example="false">Output data as csv.</param>
    /// <returns>An <see cref="ActionResult"/> for this request</returns>
    /// <response code="200">Returns a list of all users in the slot.</response>
    [HttpGet("billetinfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO[]))]
    [Produces("application/json", new[] { "text/plain" })]
    public async Task<ActionResult> BilletInfo([FromQuery(Name = "enum")] string slot, [FromQuery] string? role,
        [FromQuery(Name = "csv")] bool enableCsv)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var slotEnum = slot.ValueFromString<Slot>();
        IEnumerable<UserDTO> members;
        if (role != null)
        {
            var roleEnum = role.ValueFromString<Role>();
            members = dbContext.Users.Where(t => t.Slot == slotEnum && t.Role == roleEnum)
                .Select(t => new UserDTO()
                    { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber })
                .AsEnumerable();
        }
        else
        {
            members = dbContext.Users.Where(t => t.Slot == slotEnum)
                .Select(t => new UserDTO()
                    { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber })
                .AsEnumerable();
        }

        if (!enableCsv)
            return Ok(members.ToList());
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<UserDTO>();
        await csv.NextRecordAsync();
        await csv.WriteRecordsAsync(members);
        await csv.FlushAsync();
        await sw.FlushAsync();
        return Ok(sw.ToString());
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
    public async Task<ActionResult> CShopInfo([FromQuery(Name = "cshop")] string cShopQuery,
        [FromQuery(Name = "csv")] bool enableCsv)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var cShop = cShopQuery.ValueFromString<CShop>();
        var members = dbContext.Users.Where(t => (t.CShops & cShop) == cShop).Select(t => new UserDTO()
            { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber });
        if (!enableCsv)
            return Ok(members.ToList());
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<UserDTO>();
        await csv.NextRecordAsync();
        await csv.WriteRecordsAsync(members);
        await csv.FlushAsync();
        await sw.FlushAsync();
        return Ok(sw.ToString());
    }
}