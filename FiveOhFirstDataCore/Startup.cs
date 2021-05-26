using AngleSharp.Text;
using AspNet.Security.OpenId;
using DSharpPlus;
using FiveOhFirstDataCore.Areas.Identity;
using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Extensions;
using FiveOhFirstDataCore.Core.Mail;
using FiveOhFirstDataCore.Core.Services;
using FiveOhFirstDataCore.Core.Structures;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Secrets.GetConnectionString("database"))
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
                , ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            services.AddIdentity<Trooper, TrooperRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddRoleManager<TrooperRoleManager>()
                .AddSignInManager<TrooperSignInManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAuthentication()
                .AddSteam("Steam", "Steam", options =>
                {
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

                                    await link.BindSteamUserAsync(token, query);
                                }
                            }
                        }
                    };
                })
                .AddOAuth("Discord", "Discord", options =>
                {
                    options.AuthorizationEndpoint = $"{Configuration["Config:Discord:Api"]}/oauth2/authorize";

                    options.CallbackPath = new PathString("/authorization-code/discord-callback"); // local auth endpoint
                    options.AccessDeniedPath = new PathString("/api/link/denied");

                    options.ClientId = Configuration["Config:Discord:ClientId"];
                    options.ClientSecret = Secrets["Discord:ClientSecret"];
                    options.TokenEndpoint = $"{Configuration["Config:Discord:TokenEndpoint"]}";

                    options.Scope.Add("identify");
                    options.Scope.Add("email");

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
                                    // saved from the inital command
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
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTrooper", policy =>
                {
                    policy.RequireRole("Admin", "Manager", "Trooper");
                });

                options.AddPolicy("RequireManager", policy =>
                {
                    policy.RequireRole("Admin", "Manager");
                });

                options.AddPolicy("RequireAdmin", policy =>
                {
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("RequireSquad", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim("Slotted", "Squad");
                    });
                });

                options.AddPolicy("RequirePlatoon", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim("Slotted", "Platoon");
                    });
                });

                options.AddPolicy("RequireCompany", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim("Slotted", "Company");
                    });
                });

                options.AddPolicy("RequireNCO", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.IsInRole("NCO");
                    });
                });

                options.AddPolicy("RequireC1", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.RosterStaff].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.DocMainCom].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.RecruitingStaff].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.ReturningMemberStaff].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.MedalsStaff].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireRosterClerk", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.RosterStaff].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireNameChange", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim("Change", "Name");
                    });
                });

                options.AddPolicy("RequireRosterClerkOrReturningMemberStaff", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.RosterStaff].ContainsKey(x.Type))
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.ReturningMemberStaff].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireRecruiter", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.RecruitingStaff].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireC3", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.CampaignManagement].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.EventManagement].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireC4", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.Logistics].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireC5", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.TeamSpeakAdmin].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.HolositeSupport].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.DiscordManagement].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.TechSupport].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireC6", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.BCTStaff].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.PrivateTrainingInstructor].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.UTCStaff].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.QualTrainingStaff].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireQualificationInstructor", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.QualTrainingStaff].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireC7", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.ServerManagement].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.AuxModTeam].ContainsKey(x.Type));
                    });
                });

                options.AddPolicy("RequireC8", policy =>
                {
                    policy.RequireAssertion(ctx =>
                    {
                        return ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")
                            || ctx.User.HasClaim(x => CShopExtensions.ClaimsTree[CShop.PublicAffairs].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.MediaOutreach].ContainsKey(x.Type)
                                || CShopExtensions.ClaimsTree[CShop.NewsTeam].ContainsKey(x.Type));
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
                .AddScoped<INoticeService, NoticeService>();

            #region Discord Setup
            var cshop = Configuration.GetSection("Config:Discord:CShopRoleBindings");
            var roles = Configuration.GetSection("Config:Discord:RoleBindings");
            services.AddSingleton<DiscordConfiguration>(x => new()
            {
                Token = Secrets["Discord:Token"],
                TokenType = TokenType.Bot,
                MessageCacheSize = 0
            })
                .AddSingleton<DiscordRestClient>()
                .AddSingleton<DiscordBotConfiguration>(x => new()
                {
                    HomeGuild = ulong.Parse(Configuration["Config:Discord:HomeGuild"]),
                    CShopRoleBindings = cshop.Get<Dictionary<CShop, Dictionary<string, Dictionary<string, ulong[]>>>>(),
                    RoleBindings = roles.Get<Dictionary<string, Dictionary<string, DiscordRoleDetails>>>()
                })
                .AddScoped<IDiscordService, DiscordService>();
            #endregion
#if DEBUG
            #region Example Tools
            services.AddScoped<TestDataService>();
            #endregion
#endif            
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
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            ApplyDatabaseMigrations(db);

            var roleManager = scope.ServiceProvider.GetRequiredService<TrooperRoleManager>();
            roleManager.SeedRoles<WebsiteRoles>().GetAwaiter().GetResult();

#if DEBUG
            var data = scope.ServiceProvider.GetRequiredService<TestDataService>();
            data.InitalizeAsync().GetAwaiter().GetResult();
#endif

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

// For use in generating the inital dataset for discord role grants.
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
