using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structuresbase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public List<string> Qualifications { get; set; }
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

    public LookupController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet("userinfo")]
    [Produces("application/json")]
    public async Task<ActionResult<FullUserDTO>> UserInfo([FromQuery] int user)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var trooperQ = dbContext.Users.Where(e => e.BirthNumber == user);
        if (await trooperQ.FirstOrDefaultAsync() == null)
            return NotFound(user);
        var qualifications = Enum.GetValues<Qualification>().AsQueryable()
            .Where(x => x != Qualification.None && (trooperQ.First().Qualifications & x) == x).Select(x => x.AsFull())
            .ToList();
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
                TIB = (DateTime.Now - e.LastBilletChange).Days
            }
        ).First();
        

        return trooper;
    }

    [HttpGet("billetinfo")]
    [Produces("application/json")]
    public async Task<ActionResult<UserDTO[]>> BilletInfo([FromQuery(Name = "enum")] string slot,[FromQuery] string? role)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var slotEnum = slot.ValueFromString<Slot>();
        
        if (role != null)
        {
            var roleEnum = role.ValueFromString<Role>();
            return dbContext.Users.Where(t => t.Slot == slotEnum && t.Role == roleEnum)
                .Select(t => new UserDTO() { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber }).ToArray();
        }
        return dbContext.Users.Where(t => t.Slot == slotEnum)
            .Select(t => new UserDTO() { DisplayName = t.GetRankDesignation() + " " + t.NickName, BirthNumber = t.BirthNumber }).ToArray();
    }
}