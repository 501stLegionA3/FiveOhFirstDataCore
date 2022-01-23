using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components;
using ProjectDataCore.Data.Structures.Selector.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Data.Database;

public class ApplicationDbContext : IdentityDbContext<DataCoreUser, DataCoreRole, Guid>
{
    #region Roster
    public DbSet<RosterTree> RosterTrees { get; internal set; }
    public DbSet<RosterSlot> RosterSlots { get; internal set; }
    public DbSet<RosterDisplaySettings> RosterDisplaySettings { get; internal set; }
    public DbSet<RosterOrder> RosterOrders { get; internal set; }
    #endregion

    #region Page
    public DbSet<DisplayComponentSettings> DisplayComponentSettings { get; internal set; }
    public DbSet<EditableComponentSettings> EditableComponentSettings { get; internal set; }
    public DbSet<LayoutComponentSettings> LayoutComponentSettings { get; internal set;}
    public DbSet<CustomPageSettings> CustomPageSettings { get; internal set; }
    public DbSet<RosterComponentSettings> RosterComponentSettings { get; internal set; }
    public DbSet<ButtonComponentSettings> ButtonComponentSettings { get; internal set; }
    #endregion

    #region Forms
    public DbSet<UserSelectComponentSettings> UserSelectComponentSettings { get; internal set; }
    #endregion

    #region Assignable Values
    // Configuration Sets
    public DbSet<BaseAssignableConfiguration> AssignableConfigurations { get; internal set; }
    public DbSet<StringValueAssignableConfiguration> StringValueAssignableConfigurations { get; internal set; }
    public DbSet<IntegerValueAssignableConfiguration> IntegerValueAssignableConfigurations { get;internal set; }
    public DbSet<DoubleValueAssignableConfiguration> DoubleValueAssignableConfigurations { get; internal set; }
    public DbSet<DateTimeValueAssignableConfiguration> DateTimeValueAssignableConfigurations { get; internal set; }
    public DbSet<DateOnlyValueAssignableConfiguration> DateOnlyValueAssignableConfigurations { get; internal set; }
    public DbSet<TimeOnlyValueAssignableConfiguration> TimeOnlyValueAssignableConfigurations{ get; internal set; }

    // Assigned Value Setes
    public DbSet<BaseAssignableValue> AssignableValues { get; internal set; }
    public DbSet<StringAssignableValue> StringAssignableValues { get; internal set; }
    public DbSet<IntegerAssignableValue> IntegerAssignableValues { get; internal set; }
    public DbSet<DoubleAssignableValue> DoubleAssignableValues { get; internal set; }
    public DbSet<DateTimeAssignableValue> DateTimeAssignableValues { get; internal set; }
    public DbSet<DateOnlyAssignableValue> DateOnlyAssignableValues { get; internal set; }
    public DbSet<TimeOnlyAssignableValue> TimeOnlyAssignableValues { get; internal set; }
    #endregion

    public DbSet<NavModule> NavModules { get; internal set; }

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

        var rosterOrder = builder.Entity<RosterOrder>();
        rosterOrder.HasKey(e => e.Key);

        var rosterDisplaySettings = builder.Entity<RosterDisplaySettings>();
        rosterDisplaySettings.HasKey(e => e.Key);
        rosterDisplaySettings.HasOne(e => e.HostRoster)
            .WithMany(p => p.DisplaySettings)
            .HasForeignKey(e => e.HostRosterId);

        var rosterTree = builder.Entity<RosterTree>();
        rosterTree.HasMany(e => e.ChildRosters)
            .WithMany(p => p.ParentRosters);
        rosterTree.HasMany(e => e.RosterPositions)
            .WithOne(p => p.ParentRoster)
            .HasForeignKey(p => p.ParentRosterId);
        rosterTree.HasMany(e => e.Order)
            .WithOne(p => p.TreeToOrder)
            .HasForeignKey(p => p.TreeToOrderId);
        rosterTree.HasMany(e => e.OrderChildren)
            .WithOne(p => p.ParentObject)
            .HasForeignKey(p => p.ParentObjectId);
        rosterTree.Navigation(e => e.Order)
            .AutoInclude(true);

        var rosterSlot = builder.Entity<RosterSlot>();
        rosterSlot.HasOne(e => e.Order)
            .WithOne(p => p.SlotToOrder)
            .HasForeignKey<RosterOrder>(p => p.SlotToOrderId);
        rosterSlot.Navigation(e => e.Order)
            .AutoInclude(true);
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

        var rosterComponentSettings = builder.Entity<RosterComponentSettings>();
        rosterComponentSettings.HasMany(e => e.DefaultDisplayedProperties)
            .WithOne()
            .HasForeignKey(p => p.RosterComponentDefaultDisplayId);
        rosterComponentSettings.Navigation(e => e.DefaultDisplayedProperties)
            .AutoInclude(true);
        rosterComponentSettings.HasMany(e => e.UserListDisplayedProperties)
            .WithOne()
            .HasForeignKey(p => p.RosterComponentUserListingDisplayId);
        rosterComponentSettings.Navigation(e => e.UserListDisplayedProperties)
            .AutoInclude(true);
        rosterComponentSettings.HasMany(e => e.AvalibleRosters)
            .WithMany(p => p.DisplayComponents);
        rosterComponentSettings.Navigation(e => e.AvalibleRosters)
            .AutoInclude(true);
        #endregion

        #region Forms
        var userSelectComponentSettings = builder.Entity<UserSelectComponentSettings>();
        userSelectComponentSettings.HasKey(e => e.Key);
        userSelectComponentSettings.HasOne(e => e.LayoutComponent)
            .WithOne(p => p.UserSelectSettings)
            .HasForeignKey<UserSelectComponentSettings>(e => e.LayoutComponentId);
        #endregion

        #region Assignable Values
        var baseAssignableConfiguration = builder.Entity<BaseAssignableConfiguration>();
        baseAssignableConfiguration.HasKey(e => e.Key);

        var baseAssignableValues = builder.Entity<BaseAssignableValue>();
        baseAssignableValues.HasKey(e => e.Key);
        baseAssignableValues.HasOne(e => e.AssignableConfiguration)
            .WithMany(p => p.AssignableValues)
            .HasForeignKey(e => e.AssignableConfigurationId);
        baseAssignableValues.HasOne(e => e.ForUser)
            .WithMany(p => p.AssignableValues)
            .HasForeignKey(e => e.ForUserId);
        baseAssignableValues.Navigation(e => e.AssignableConfiguration)
            .AutoInclude();
        #endregion

        #region User
        var dataCoreUser = builder.Entity<DataCoreUser>();
        dataCoreUser.HasMany(e => e.RosterSlots)
            .WithOne(e => e.OccupiedBy)
            .HasForeignKey(e => e.OccupiedById);
        dataCoreUser.Navigation(e => e.AssignableValues)
            .AutoInclude();
            
        var dataCoreUserProperty = builder.Entity<DataCoreUserProperty>();
        dataCoreUserProperty.HasKey(e => e.Key);
        #endregion

        #region Nav Modules

        var navModules = builder.Entity<NavModule>();
        navModules.HasKey(e => e.Key);
        navModules.HasMany(e => e.SubModules)
            .WithOne(e => e.Parent)
            .HasForeignKey(e => e.ParentId);

        #endregion

    }
}