using AngleSharp.Text;

using AspNet.Security.OpenId;

using DSharpPlus;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Diagnostics;

using ProjectDataCore.Data.Services.Account;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Import;
using ProjectDataCore.Data.Services.InternalAuth;
using ProjectDataCore.Data.Services.Logging;
using ProjectDataCore.Data.Services.Mail;
using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Services.User;
using ProjectDataCore.Data.Structures.Mail;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        #region Database Setup

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql(
                Configuration.GetConnectionString("database"))
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

        services.AddScoped(p
            => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        #endregion

        #region Accounts

        services.AddIdentity<DataCoreUser, DataCoreRole>()
            .AddDefaultTokenProviders()
            .AddRoleManager<DataCoreRoleManager>()
            .AddSignInManager<DataCoreSignInManager>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        #endregion

        #region Blazor

        services.AddRazorPages();
        services.AddServerSideBlazor();

#if DEBUG
        services.AddDatabaseDeveloperPageExceptionFilter();
#endif

        #endregion

        #region Password

        services.Configure<IdentityOptions>(options =>
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
        services.AddScoped<IModularRosterService, ModularRosterService>()
            .AddScoped<IPageEditService, PageEditService>()
            .AddScoped<IScopedUserService, ScopedUserService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<INavModuleService, NavModuleService>()
            .AddScoped<IAssignableDataService, AssignableDataService>()
            .AddScoped<IAlertService, AlertService>()
            .AddScoped<IImportService, ImportService>();

        // Scoped Services
        services
            .AddScoped<ILocalUserService, LocalUserService>()
            .AddScoped<IUserService, UserService>();

        // Singleton Services
        services.AddSingleton<IRoutingService, RoutingService>()
            .AddSingleton<RoutingService.RoutingServiceSettings>(x =>
            {
                var asm = Assembly.GetAssembly(typeof(Components._Imports));

                if (asm is null)
                    throw new Exception("Missing components assembly.");

                return new(asm);
            })
            .AddSingleton<IAccountLinkService, AccountLinkService>()
            .AddSingleton<IPolicyService, PolicyService>()
            .AddSingleton<IDataBus, DataBus>()
            .AddSingleton<IModularRosterService, ModularRosterService>()
            .AddSingleton<IPageEditService, PageEditService>()
            .AddSingleton<IScopedUserService, ScopedUserService>()
            .AddSingleton<INavModuleService, NavModuleService>()
            .AddSingleton<IAssignableDataService, AssignableDataService>()
            .AddSingleton<IInternalAuthorizationService, InternalAuthorizationService>()
            .AddSingleton<IInstanceLogger, InstanceLogger>();

        #endregion

        #region Email Setup
        var mailSection = Configuration.GetRequiredSection("Email");
        services.AddSingleton(new MailConfiguration()
        {
            Client = mailSection["Client"],
            Port = mailSection["Port"].ToInteger(0),
            Email = mailSection["Email"],
            RequireLogin = mailSection["RequireLogin"].ToBoolean(false),
            User = mailSection["User"],
            Password = mailSection["Password"]
        })
            .AddScoped<SmtpClient>()
            .AddScoped<ICustomMailSender, CustomMailSender>();
        #endregion

        #region Discord Setup

        #endregion

        #region Authentication

        services.AddAuthentication()
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
                options.AuthorizationEndpoint = $"{Configuration["Config:Discord:Api"]}/oauth2/authorize";

                options.CallbackPath = new PathString("/authorization-code/discord-callback"); // local auth endpoint
                options.AccessDeniedPath = new PathString("/api/link/denied");

                options.ClientId = Configuration["Discord:ClientId"];
                options.ClientSecret = Configuration["Discord:ClientSecret"];
                options.TokenEndpoint = $"{Configuration["Config:Discord:TokenEndpoint"]}";

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
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
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

        Task.Run(async () =>
        {
            #region Database Validation
            using var scope = app.ApplicationServices.CreateScope();
            var dbFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var db = await dbFac.CreateDbContextAsync();
            ApplyDatabaseMigrations(db);
            #endregion

            #region File Validation
            var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var unsafeUploadsPath = Path.Combine(webHostEnvironment.ContentRootPath,
                webHostEnvironment.EnvironmentName, "unsafe_uploads");

            Directory.CreateDirectory(unsafeUploadsPath);
            #endregion

            #region Admin Validation

            var usrMngr = scope.ServiceProvider.GetRequiredService<UserManager<DataCoreUser>>();
            var usr = await usrMngr.FindByNameAsync("Administrator");
            bool userGenerated = false;
            if (usr is null)
            {
                usr = new()
                {
                    UserName = "Administrator",
                    DiscordId = 0,
                    SteamLink = "I like your funny words, magic man"
                };

                await usrMngr.CreateAsync(usr, Configuration["Startup:Password"]);
                var usrServ = scope.ServiceProvider.GetRequiredService<IAssignableDataService>();

                usr = await usrMngr.FindByNameAsync("Administrator");
                await usrServ.EnsureAssignableValuesAsync(usr);

                userGenerated = true;
            }

            var adminPolicy = await db.DynamicAuthorizationPolicies
                .Where(x => x.AdminPolicy)
                .FirstOrDefaultAsync();

            if (adminPolicy is null)
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

            #region NavBar Validation
            var navCount = await db.NavModules.CountAsync();
            var navSrvc = scope.ServiceProvider.GetRequiredService<INavModuleService>();
            if (navCount == 0)
            {
                NavModule admin = new()
                {
                    DisplayName = "Admin",
                    Href = "admin",
                    HasMainPage = true,
                    IsAdmin = true,
                };
                await navSrvc.CreateNavModuleAsync(admin);
                await navSrvc.CreateNavModuleAsync("Edit Nav Bar", "admin/navbar/edit", true, admin.Key);
                await navSrvc.CreateNavModuleAsync("Page Editor", "admin/page/edit", true, admin.Key);
                await navSrvc.CreateNavModuleAsync("Roster Editor", "admin/roster/edit", true, admin.Key);
                NavModule account = new()
                {
                    DisplayName = "Account",
                    Href = "admin/account",
                    HasMainPage = true,
                    ParentId = admin.Key
                };
                await navSrvc.CreateNavModuleAsync(account);
                await navSrvc.CreateNavModuleAsync("Assignable Value Editor", "admin/account/avedit", true, account.Key);
                await navSrvc.CreateNavModuleAsync("Policy Editor", "admin/policy/dpedit", true, admin.Key);
            }
            #endregion
        }).GetAwaiter().GetResult();

        app.UseForwardedHeaders(new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        //app.UseHttpsRedirection();
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
    }

    private static void ApplyDatabaseMigrations(DbContext database)
    {
        if (!(database.Database.GetPendingMigrations()).Any())
        {
            return;
        }

        database.Database.Migrate();
        database.SaveChanges();
    }
}
