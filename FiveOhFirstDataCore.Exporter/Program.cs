using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structuresbase;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var Secrets = new ConfigurationBuilder()
                .AddJsonFile(Path.Join("Config", "website_config.json"))
                .AddJsonFile("appsettings.json")
                .Build();

var services = new ServiceCollection();

services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(
        Secrets.GetConnectionString("database"))
    , ServiceLifetime.Singleton);

services.AddLogging(o => o.AddConsole()
    .SetMinimumLevel(LogLevel.Debug)
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning))
    .AddSingleton<IConfiguration>(Secrets);

var provider = services.BuildServiceProvider();

try
{
    // Run the program
    Execute(provider).GetAwaiter().GetResult();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine();
    Console.WriteLine(ex.StackTrace);
}
finally
{
    // Wait at the end
    Console.ReadLine();
}

static async Task Execute(IServiceProvider provider)
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    var dbContextFactory = provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    var dbContext = dbContextFactory.CreateDbContext();

    logger.LogInformation("Starting import process.\nBuilding Exports folder...");
    
    Directory.CreateDirectory("Exports");

    logger.LogInformation("Starting information scrapping");

    var users = dbContext.Users
        .Include(x => x.RecruitStatus)
        .Include(x => x.Descriptions)
        .AsNoTracking()
        .AsAsyncEnumerable();

    List<string[]> dataRows = new();

    // Row Data
    dataRows.Add(new string[]
    {
        // General Information
        "Username",
        "Password Hash",
        "Email",
        "Nickname",
        "Birth Number",
        // Trooper Info
        "Rank Aggergate",
        "C-Shops",
        "Qualifications",
        "Last Promotion",
        "Start Of Service",
        "Last Billet Change",
        "BCT Graduation Date",
        "UTC Graduation Date",
        "Notes Aggergate",
        "Discord ID",
        "Steam Link",
        "Access Code",
        // Recruit Information
        "Over Sixteen",
        "Age",
        "Mods Installed",
        "Possible Troll",
        "Preferred Role",
        // Util
        "Last Update",
        "Roster Aggergate"
    });

    await foreach(var user in users)
    {
        string username;
        string passwordHash;
        string email;
        string nickname;
        string birthNumber;
        string rankAggergate;
        string cshops;
        string qualifications;
        string lastPromotion;
        string startOfService;
        string lastBilletChange;
        string BCTGradDate;
        string UTCGradDate;
        string notesAggergate;
        string discordId;
        string steamLink;
        string accessCode;
        string overSixteen;
        string age;
        string modsInstalled;
        string possibleTroll;
        string preferredRole;
        string lastUpdate;
        string rosterAggergate;

        using (logger.BeginScope("Started Processing User {User}", user.UserName))
        {
            logger.LogDebug("Running log for {User}", user);

            username = user.UserName;
            passwordHash = user.PasswordHash;
            email = user.Email;
            nickname = user.NickName;
            birthNumber = user.BirthNumber.ToString();

            rankAggergate = user.GetRankName();
            rankAggergate += ";" + user.Rank?.AsFull();

            List<string> cshopItems = new();
            foreach (CShop shop in Enum.GetValues<CShop>())
                if ((user.CShops & shop) == shop)
                    cshopItems.Add(shop.AsFull());
            cshops = string.Join(";", cshopItems);

            List<string> qualItems = new();
            foreach (Qualification qual in Enum.GetValues<Qualification>())
                if ((user.Qualifications & qual) == qual)
                    qualItems.Add(qual.AsFull());
            qualifications = string.Join(";", qualItems);

            lastPromotion = user.LastPromotion.ToString();
            startOfService = user.StartOfService.ToString();
            lastBilletChange = user.LastBilletChange.ToString();
            BCTGradDate = user.GraduatedBCTOn.ToString();
            UTCGradDate = user.GraduatedUTCOn.ToString();

            List<string> notesItems = new();
            if (!string.IsNullOrWhiteSpace(user.Notes))
                notesItems.Add(user.Notes);
            if (!string.IsNullOrWhiteSpace(user.InitialTraining))
                notesItems.Add(user.InitialTraining);
            if (!string.IsNullOrWhiteSpace(user.UTC))
                notesItems.Add(user.UTC);
            notesAggergate = String.Join(";", notesItems);

            discordId = user.DiscordId ?? "";
            steamLink = user.SteamLink ?? "";
            accessCode = user.AccessCode ?? "";

            overSixteen = user.RecruitStatus?.OverSixteen.ToString() ?? "";
            age = user.RecruitStatus?.Age.ToString() ?? "";
            modsInstalled = user.RecruitStatus?.ModsInstalled.ToString() ?? "";
            possibleTroll = user.RecruitStatus?.PossibleTroll.ToString() ?? "";
            preferredRole = user.RecruitStatus?.PreferredRole.AsFull() ?? "";

            lastUpdate = user.LastUpdate.ToString();

            List<string> rosterParts = new()
            {
                user.Slot.AsFull(),
                user.Role.AsFull(),
                user.Team?.AsFull() ?? "",
                user.Flight?.AsFull() ?? ""
            };
            rosterAggergate = string.Join(";", rosterParts);

            var values = new string[]
            {
                username,
                passwordHash,
                email,
                nickname,
                birthNumber,
                rankAggergate,
                cshops,
                qualifications,
                lastPromotion,
                startOfService,
                lastBilletChange,
                BCTGradDate,
                UTCGradDate,
                notesAggergate,
                discordId,
                steamLink,
                accessCode,
                overSixteen,
                age,
                modsInstalled,
                possibleTroll,
                preferredRole,
                lastUpdate,
                rosterAggergate
            };
            
            for (int z = 0; z < values.Length; z++)
            {
                logger.LogDebug("{Item}: {Value}", z, values[z]);
                if (!string.IsNullOrEmpty(values[z]))
                {
                    values[z] = values[z].Replace(',', ';');
                    values[z] = values[z].ReplaceLineEndings(";");
                }
            }

            dataRows.Add(values);
        }
    }

    logger.LogInformation("Finished data gathering. Starting data export...");

    var path = Path.Combine("Exports", $"export_data_{DateTime.Now.ToString("MM-dd-yyy_hh-mm-ss")}.csv");
    await using FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
    await using StreamWriter sw = new(fs);

    foreach (var row in dataRows)
        await sw.WriteLineAsync(string.Join(",", row));

    logger.LogInformation($"Wrote {dataRows.Count} rows to {path}");
}

