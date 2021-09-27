
using AngleSharp.Text;

using AspNet.Security.OpenId;

using DSharpPlus;

using FiveOhFirstDataCore.Areas.Identity;
using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Mail;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace FiveOhFirstDataCore
{
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
            var Secrets = new ConfigurationBuilder()
                .AddJsonFile(Path.Join("Config", "website_config.json"))
                .Build();

            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Secrets.GetConnectionString("database"))
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                , ServiceLifetime.Singleton);
            services.AddScoped(p
                => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
            services.AddIdentity<Trooper, TrooperRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddRoleManager<TrooperRoleManager>()
                .AddSignInManager<TrooperSignInManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<Trooper>>();
#if DEBUG
            services.AddDatabaseDeveloperPageExceptionFilter();
#endif

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

                                    var link = context.HttpContext.RequestServices.GetRequiredService<AccountLinkService>();

                                    string id = query;
                                    id = id[(id.LastIndexOf("/") + 1)..(id.Length)];

                                    try
                                    {
                                        await link.BindSteamUserAsync(token, id);
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

                    options.ClientId = Secrets["Discord:ClientId"];
                    options.ClientSecret = Secrets["Discord:ClientSecret"];
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
                                var link = context.HttpContext.RequestServices.GetRequiredService<AccountLinkService>();

                                try
                                {
                                    // verify the user information grabbed matches the user info
                                    // saved from the Initial command
                                    await link.BindDiscordAsync(token, user.Id, user.Email);
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
                                var link = context.HttpContext.RequestServices.GetRequiredService<AccountLinkService>();
                                await link.AbortLinkAsync(token);
                            }
                        }
                    };
                });

            #region Policy Setup
            services.AddSingleton<IAuthorizationPolicyProvider, DataCoreAuthorizationPolicyProvider>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTrooper", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        return ctx.User.IsInRole("Trooper");
                    });
                });

                options.AddPolicy("RequireManager", policy =>
                {
                    policy.RequireRole("Admin", "Manager");
                });

                options.AddPolicy("RequireAdmin", policy =>
                {
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("RequireNCO", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        return ctx.User.IsInRole("NCO")
                            || ctx.User.HasClaim(x => x.Type == "Grant" && x.Value == "NCO");
                    });
                });

                options.AddPolicy("RequireNameChange", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim("Change", "Name");
                    });
                });
            });
            #endregion

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 2;
                options.Password.RequiredUniqueChars = 0;
            });

            var mailSection = Secrets.GetRequiredSection("Email");
            services.AddSingleton<AccountLinkService>()
                .AddScoped<IRefreshRequestService, RefreshRequestService>()
                .AddScoped<IRosterService, RosterService>()
                .AddScoped<INicknameComparisonService, NicknameComparisionService>()
                .AddScoped<IUpdateService, UpdateService>()
                .AddSingleton(new MailConfiguration()
                {
                    Client = mailSection["Client"],
                    Port = mailSection["Port"].ToInteger(0),
                    Email = mailSection["Email"],
                    RequireLogin = mailSection["RequireLogin"].ToBoolean(false),
                    User = mailSection["User"],
                    Password = mailSection["Password"]
                })
                .AddScoped<SmtpClient>()
                .AddScoped<ICustomMailSender, MailSender>()
                .AddScoped<IImportService, ImportService>()
                .AddScoped<INoticeService, NoticeService>()
                .AddScoped<IPromotionService, PromotionService>()
                .AddScoped<IEparService, EparService>()
                .AddScoped<IReportService, ReportService>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IMessageService, MessageService>()
                .AddScoped<IAlertService, AlertService>()
                .AddSingleton<IWebsiteSettingsService, WebsiteSettingsService>()
                .AddSingleton<IAdvancedRefreshService, AdvancedRefreshService>();

            #region Discord Setup
            services.AddSingleton<DiscordConfiguration>(x => new()
            {
                Token = Secrets["Discord:Token"],
                TokenType = TokenType.Bot,
                MessageCacheSize = 0
            })
                .AddSingleton<DiscordRestClient>()
                .AddSingleton<DiscordBotConfiguration>(x => new()
                {
                    HomeGuild = ulong.Parse(Secrets["Discord:HomeGuild"])
                })
                .AddScoped<IDiscordService, DiscordService>();
            #endregion

            #region Account Tools
            services.AddScoped<InitalAccountPopulationService>();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            using var scope = app.ApplicationServices.CreateScope();
            var dbFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var db = dbFac.CreateDbContext();
            ApplyDatabaseMigrations(db);

            // Validate birth numbers.
            db.Users.Where(x => x.BirthNumber == 0)
                .ForEachAsync(x => x.BirthNumber = x.Id).GetAwaiter().GetResult();
            db.SaveChanges();

            var roleManager = scope.ServiceProvider.GetRequiredService<TrooperRoleManager>();
            roleManager.SeedRoles<WebsiteRoles>().GetAwaiter().GetResult();

            var data = scope.ServiceProvider.GetRequiredService<InitalAccountPopulationService>();
            data.InitializeAsync().GetAwaiter().GetResult();

            var claims = scope.ServiceProvider.GetRequiredService<IWebsiteSettingsService>();
            claims.InitalizeAsync().GetAwaiter().GetResult();

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
}

// For use in generating the Initial dataset for discord role grants.
// 
//var types = new Type[] {typeof(Role), typeof(Slot), typeof(Team), typeof(Flight), typeof(TrooperRank),
//typeof(RTORank), typeof(MedicRank), typeof(PilotRank), typeof(WardenRank), typeof(WarrantRank), typeof(Qualification)};
//var dict = new Dictionary<string, Dictionary<Enum, DiscordRoleDetails>>();
//foreach (var t in types)
//{
//    var name = t.Name;
//    dict.Add(name, new());
//    foreach(Enum val in Enum.GetValues(t))
//    {
//        dict[name].Add(val, new()
//        {
//            RoleGrants = new ulong[] { 0 },
//            RoleReplaces = new ulong[] { 0 }
//        });
//    }
//}

//File.WriteAllText(Path.Join("Config", "test_set.json"), 
//    JsonConvert.SerializeObject(dict, Formatting.Indented));
