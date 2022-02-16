using AspNet.Security.OpenId;

using DSharpPlus;

using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using ProjectDataCore.Data.Database;
using ProjectDataCore.Data.Policy;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Account;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(Path.Join("Config", "website_config.json"));

#region Database Setup

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("database"))
#if DEBUG
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
#endif
    // Warnings for loading multiple collections without splitting the query.
    // We want this behaviour for accurate data loading (and our lists are not
    // of any large size for the roster) so we are ignoring these.
    .ConfigureWarnings(w =>
        w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning)
    ), ServiceLifetime.Singleton);

builder.Services.AddScoped(p
    => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

#endregion

#region Accounts

builder.Services.AddIdentity<DataCoreUser, DataCoreRole>()
    .AddDefaultTokenProviders()
    .AddRoleManager<DataCoreRoleManager>()
    .AddSignInManager<DataCoreSignInManager>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

#endregion

#region Blazor

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<DataCoreUser>>();

#if DEBUG
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
#endif

#endregion

#region Password

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 2;
    options.Password.RequiredUniqueChars = 0;
});

#endregion

#region Services
builder.Services.AddScoped<IModularRosterService, ModularRosterService>()
    .AddScoped<IPageEditService, PageEditService>()
    .AddScoped<IScopedUserService, ScopedUserService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<INavModuleService, NavModuleService>()
    .AddScoped<IAssignableDataService, AssignableDataService>()
    .AddScoped<IAlertService, AlertService>();

// Scoped Services
builder.Services
    .AddScoped<ILocalUserService, LocalUserService>()
    .AddScoped<IUserService, UserService>();

// Singleton Services
builder.Services.AddSingleton<IRoutingService, RoutingService>()
    .AddSingleton<RoutingService.RoutingServiceSettings>(x =>
    {
        var asm = Assembly.GetAssembly(typeof(ProjectDataCore.Components._Imports));

        if (asm is null)
            throw new Exception("Missing components assembly.");

        return new(asm);
    })
    .AddSingleton<IAccountLinkService, AccountLinkService>()
    .AddSingleton<IPolicyService, PolicyService>()
    .AddSingleton<IAuthorizationPolicyProvider, DataCoreAuthorizationPolicyProvider>()
    .AddSingleton<IDataBus, DataBus>()
    .AddSingleton<IModularRosterService, ModularRosterService>()
    .AddSingleton<IPageEditService, PageEditService>()
    .AddSingleton<IScopedUserService, ScopedUserService>()
    .AddSingleton<INavModuleService, NavModuleService>()
    .AddSingleton<IAssignableDataService, AssignableDataService>();

#endregion

#region Discord Setup

#endregion

#region Authentication

builder.Services.AddAuthentication()
    .AddSteam("Steam", "Steam", options =>
    {
        options.CorrelationCookie = new()
        {
            IsEssential = true,
            SameSite = SameSiteMode.None,
            SecurePolicy = CookieSecurePolicy.Always
        };

        options.Events = new OpenIdAuthenticationEvents
        {
            OnTicketReceived = async context =>
            {
                if (context.Request.Cookies.TryGetValue("token", out var token))
                {
                    if (context.Request.QueryString.HasValue)
                    {
                        var query = context.Request.Query["openid.identity"];

                        var link = context.HttpContext.RequestServices.GetRequiredService<IAccountLinkService>();

                        string id = query;
                        id = id[(id.LastIndexOf("/") + 1)..(id.Length)];

                        try
                        {
                            var redir = await link.BindSteamUserAsync(token!, id);

                            context.Response.Cookies.Append("redir", redir);
                        }
                        catch (Exception ex)
                        {
                            context.Response.Cookies.Append("error", ex.Message);
                        }

                        context.Success();
                    }
                }
                else
                {
                    context.Response.Cookies.Append("error", "No token was found.");
                }
            }
        };
    })
    .AddOAuth("Discord", "Discord", options =>
    {
        options.AuthorizationEndpoint = $"{builder.Configuration["Config:Discord:Api"]}/oauth2/authorize";

        options.CallbackPath = new PathString("/authorization-code/discord-callback"); // local auth endpoint
        options.AccessDeniedPath = new PathString("/api/link/denied");

        options.ClientId = builder.Configuration["Discord:ClientId"];
        options.ClientSecret = builder.Configuration["Discord:ClientSecret"];
        options.TokenEndpoint = $"{builder.Configuration["Config:Discord:TokenEndpoint"]}";

        options.Scope.Add("identify");
        options.Scope.Add("email");

        options.CorrelationCookie = new()
        {
            IsEssential = true,
            SameSite = SameSiteMode.None,
            SecurePolicy = CookieSecurePolicy.Always
        };

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                // get user data
                var client = new DiscordRestClient(new()
                {
                    Token = context.AccessToken,
                    TokenType = TokenType.Bearer
                });

                var user = await client.GetCurrentUserAsync();

                if (context.Request.Cookies.TryGetValue("token", out var token))
                {
                    var link = context.HttpContext.RequestServices.GetRequiredService<IAccountLinkService>();

                    try
                    {
                        // verify the user information grabbed matches the user info
                        // saved from the Initial command
                        var redir = await link.BindDiscordAsync(token, user.Id, user.Email);

                        context.Response.Cookies.Append("redir", redir);
                    }
                    catch (Exception ex)
                    {
                        context.Response.Cookies.Append("error", ex.Message);
                    }

                    context.Success();
                }
                else
                {
                    context.Response.Cookies.Append("error", "No token found.");
                }
            },
            OnRemoteFailure = async context =>
            {
                // TODO remove token from cookies and delete server token cache.
                if (context.Request.Cookies.TryGetValue("token", out var token))
                {
                    var link = context.HttpContext.RequestServices.GetRequiredService<IAccountLinkService>();
                    await link.AbortLinkAsync(token!);
                }
            }
        };
    });

#endregion

#region Permissions

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using var scope = app.Services.CreateScope();
var dbFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
await using var db = await dbFac.CreateDbContextAsync();
ApplyDatabaseMigrations(db);

#region Admin Validation

var usrMngr = scope.ServiceProvider.GetRequiredService<UserManager<DataCoreUser>>();
var usr = await usrMngr.FindByNameAsync("Administrator");
bool userGenerated = false;
if(usr is null)
{
    usr = new()
    {
        UserName = "Administrator",
        DiscordId = 0,
        SteamLink = "I like your funny words, magic man"
    };

    await usrMngr.CreateAsync(usr, app.Configuration["Startup:Password"]);
    var usrServ = scope.ServiceProvider.GetRequiredService<IAssignableDataService>();

    usr = await usrMngr.FindByNameAsync("Administrator");
    await usrServ.EnsureAssignableValuesAsync(usr);

    userGenerated = true;
}

var adminPolicy = await db.DynamicAuthorizationPolicies
    .Where(x => x.AdminPolicy)
    .FirstOrDefaultAsync();

if(adminPolicy is null)
{
    adminPolicy = new()
    {
        AdminPolicy = true,
        PolicyName = "Administrator Policy"
    };

    adminPolicy.AdministratorPolicy = adminPolicy;

    await db.AddAsync(adminPolicy);
    await db.SaveChangesAsync();
}

if (userGenerated)
{
    await db.Entry(usr).ReloadAsync();

    adminPolicy.AuthorizedUsers.Add(usr);
    await db.SaveChangesAsync();
}

#endregion

app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();

static void ApplyDatabaseMigrations(DbContext database)
{
    if (!(database.Database.GetPendingMigrations()).Any())
    {
        return;
    }

    database.Database.Migrate();
    database.SaveChanges();
}