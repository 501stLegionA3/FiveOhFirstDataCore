using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Database;

public class ApplicationDbContext : IdentityDbContext<DataCoreUser, DataCoreRole, int>
{
    #region Roster
    public DbSet<RosterTree> RosterTrees { get; internal set; }
    public DbSet<RosterSlot> RosterSlots { get; internal set; }
    public DbSet<RosterDisplaySettings> RosterDisplaySettings { get; internal set; }
    public DbSet<RosterParentLink> RosterParentLinks { get; internal set; }
    #endregion

    #region Page
    public DbSet<DisplayComponentSettings> DisplayComponentSettings { get; internal set; }
    public DbSet<EditableComponentSettings> EditableComponentSettings { get; internal set; }
    public DbSet<LayoutComponentSettings> LayoutComponentSettings { get; internal set;}
    public DbSet<CustomPageSettings> CustomPageSettings { get; internal set; }
    #endregion

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Roster
        var rosterObject = builder.Entity<RosterObject>();
        rosterObject.HasKey(e => e.Key);

        var rosterParentLink = builder.Entity<RosterParentLink>();
        rosterParentLink.HasKey(e => e.Key);

        var rosertDisplaySettings = builder.Entity<RosterDisplaySettings>();
        rosertDisplaySettings.HasKey(e => e.Key);
        #endregion

        #region Page
        var customPageSettings = builder.Entity<CustomPageSettings>();
        customPageSettings.HasKey(e => e.Key);
        customPageSettings.HasOne(e => e.Layout)
            .WithOne(p => p.ParentPage)
            .HasForeignKey<CustomPageSettings>(e => e.LayoutId);
        customPageSettings.HasIndex(e => e.Route)
            .IsUnique(true);

        var layoutComponentSettings = builder.Entity<LayoutComponentSettings>();
        layoutComponentSettings.HasOne(e => e.ParentPage)
            .WithOne(p => p.Layout)
            .HasForeignKey<LayoutComponentSettings>(p => p.ParentPageId);

        var pageComponentSettingsBase = builder.Entity<PageComponentSettingsBase>();
        pageComponentSettingsBase.HasKey(e => e.Key);
        pageComponentSettingsBase.HasOne(e => e.ParentLayout)
            .WithMany(p => p.ChildComponents)
            .HasForeignKey(e => e.ParentLayoutId);

        var parameterComponentSettings = builder.Entity<ParameterComponentSettingsBase>();
        parameterComponentSettings.HasOne(e => e.UserScope)
            .WithMany(p => p.AttachedScopes)
            .HasForeignKey(e => e.UserScopeId);
        #endregion

        #region User
        var dataCoreUser = builder.Entity<DataCoreUser>();
        dataCoreUser.HasMany(e => e.RosterSlots)
            .WithOne(e => e.OccupiedBy)
            .HasForeignKey(e => e.OccupiedById);
            
        #endregion
    }
}
