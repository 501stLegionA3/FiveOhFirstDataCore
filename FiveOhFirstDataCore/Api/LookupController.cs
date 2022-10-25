using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Updates;
using FiveOhFirstDataCore.Data.Structuresbase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using CsvHelper;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FiveOhFirstDataCore.Api;

public class FullUserDTO
{
    public string NickName { get; set; }
    public int BirthNumber { get; set; }
    public DateTime LastPromotion { get; set; }
    public DateTime LastBilletChange { get; set; }
    public string Slot { get; set; }
    public string Role { get; set; }
    public string Rank { get; set; }
    public string Qualifications { get; set; }
    public string CShops { get; set; }
    public int TIG { get; set; }
    public int TIB { get; set; }
}

public class UserDTO
{
    public string DisplayName { get; set; }
    public int BirthNumber { get; set; }
}

[Route("api")]
[ApiController]
public class LookupController : ControllerBase
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IRosterService _roster;

    public LookupController(IDbContextFactory<ApplicationDbContext> dbContextFactory, IRosterService roster)
    {
        _dbContextFactory = dbContextFactory;
        _roster = roster;
    }

    [HttpGet("userinfo")]
    [Produces("text/plain")]
    public async Task<ActionResult<string>> UserInfo([FromQuery] int user)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var trooperQ = dbContext.Users.Where(e => e.BirthNumber == user);
        if ((await trooperQ.FirstOrDefaultAsync()) == null)
            return NotFound(user.ToString());
        var qualificationsQ = Enum.GetValues<Qualification>().AsQueryable()
            .Where(x => x != Qualification.None && (trooperQ.First().Qualifications & x) == x).Select(x => x.AsFull());
        var qualifications = "";
        if (qualificationsQ.Any())
             qualifications = qualificationsQ.Aggregate((a, b) => a + ", " + b);
        var ShopPositionsQ = Enum.GetValues<CShop>().AsQueryable()
            .Where(x => x != CShop.None && (trooperQ.First().CShops & x) == x).Select(x => x.AsFull());
        var ShopPositions = "";
        if (ShopPositionsQ.Any())
            ShopPositions = ShopPositionsQ.Aggregate((a, b) => a + ", " + b);
        var trooper = trooperQ.Select(e =>
            new FullUserDTO()
            {
                NickName = e.NickName,
                BirthNumber = e.BirthNumber,
                LastPromotion = e.LastPromotion,
                LastBilletChange = e.LastBilletChange,
                Slot = e.Slot.AsFull(),
                Role = e.GetRoleName(),
                Rank = e.GetRankName(),
                Qualifications = qualifications,
                TIG = (DateTime.Now - e.LastPromotion).Days,
                TIB = (DateTime.Now - e.LastBilletChange).Days,
                CShops = ShopPositions
            }
        ).First();
        
        string jsonString = JsonSerializer.Serialize(trooper);

        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<FullUserDTO>();
        await csv.NextRecordAsync();
        csv.WriteRecord(trooper);
        await csv.FlushAsync();

        return sw.ToString();
    }

    [HttpGet("billetinfo")]
    [Produces("text/plain")]
    public async Task<ActionResult<string>> BilletInfo([FromQuery(Name = "enum")] string slot,[FromQuery] string? role)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var slotEnum = slot.ValueFromString<Slot>();
        IEnumerable<UserDTO> members;
        if (role != null)
        {
            var roleEnum = role.ValueFromString<Role>();
            members = dbContext.Users.Where(t => t.Slot == slotEnum && t.Role == roleEnum)
                .Select(t => new UserDTO() { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber }).AsEnumerable();
        }
        else
        {
            members = dbContext.Users.Where(t => t.Slot == slotEnum)
                .Select(t => new UserDTO()
                    { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber }).AsEnumerable();
        }
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<UserDTO>();
        await csv.NextRecordAsync();
        await csv.WriteRecordsAsync(members);
        await csv.FlushAsync();
        await sw.FlushAsync();
        return sw.ToString();
    }

    [HttpGet("cshopinfo")]
    [Produces("text/plain")]
    public async Task<ActionResult<string>> CShopInfo([FromQuery(Name = "cshop")] string cShopQuery)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var cShop = cShopQuery.ValueFromString<CShop>();
        var members = dbContext.Users.Where(t => (t.CShops & cShop) == cShop).Select(t => new UserDTO() { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber });
        await using var sw = new StringWriter();
        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.WriteHeader<UserDTO>();
        await csv.NextRecordAsync();
        await csv.WriteRecordsAsync(members);
        await csv.FlushAsync();
        await sw.FlushAsync();
        return sw.ToString();
    }
}