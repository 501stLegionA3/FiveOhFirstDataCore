using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using ProjectDataCore.Data.Structures.Account;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Render;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Keybindings;
using ProjectDataCore.Data.Structures.Nav;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components.Layout;
using ProjectDataCore.Data.Structures.Page.Components.Parameters;
using ProjectDataCore.Data.Structures.Page.Components.Scope;
using ProjectDataCore.Data.Structures.Policy;
using ProjectDataCore.Data.Structures.Selector.User;

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
    public DbSet<LayoutNode> LayoutNodes { get; internal set; }
    public DbSet<EditableComponentSettings> EditableComponentSettings { get; internal set; }
    public DbSet<CustomPageSettings> CustomPageSettings { get; internal set; }
    public DbSet<RosterComponentSettings> RosterComponentSettings { get; internal set; }
    public DbSet<ButtonComponentSettings> ButtonComponentSettings { get; internal set; }
    public DbSet<TextDisplayComponentSettings> TextDisplayComponentSettings { get; internal set; }
    public DbSet<UserScope> UserScopes { get; internal set; }
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
    public DbSet<BooleanValueAssignableConfiguration> BooleanValueAssignableConfigurations { get; internal set; }

    // Assigned Value Setes
    public DbSet<BaseAssignableValue> AssignableValues { get; internal set; }
    public DbSet<StringAssignableValue> StringAssignableValues { get; internal set; }
    public DbSet<IntegerAssignableValue> IntegerAssignableValues { get; internal set; }
    public DbSet<DoubleAssignableValue> DoubleAssignableValues { get; internal set; }
    public DbSet<DateTimeAssignableValue> DateTimeAssignableValues { get; internal set; }
    public DbSet<DateOnlyAssignableValue> DateOnlyAssignableValues { get; internal set; }
    public DbSet<TimeOnlyAssignableValue> TimeOnlyAssignableValues { get; internal set; }
    public DbSet<BooleanAssignableValue> BooleanAssignableValues { get; internal set; }

    // Assignable Value Rendering
    public DbSet<AssignableValueRenderer> AssignableValueRenderers { get; internal set; }
    public DbSet<AssignableValueConversion> AssignableValueConversions { get; internal set; }
    #endregion

    #region Account Link
    public DbSet<AccountSettings> LinkSettings { get; internal set; }
    #endregion

    #region Nav Modules
    public DbSet<NavModule> NavModules { get; internal set; }
    #endregion

    #region Policy
    public DbSet<DynamicAuthorizationPolicy> DynamicAuthorizationPolicies { get; internal set; }
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
        rosterSlot.Navigation(e => e.OccupiedBy)
            .AutoInclude(true);
        #endregion

        #region Page
        var customPageSettings = builder.Entity<CustomPageSettings>();
        customPageSettings.HasKey(e => e.Key);
        customPageSettings.HasOne(e => e.Layout)
            .WithOne(p => p.PageSettings)
            .HasForeignKey<LayoutNode>(e => e.PageSettingsId);
        customPageSettings.HasMany(e => e.UserScopes)
            .WithOne(p => p.Page)
            .HasForeignKey(p => p.PageId);
        customPageSettings.HasIndex(e => e.Route)
            .IsUnique(true);

        var layoutNodes = builder.Entity<LayoutNode>();
        layoutNodes.HasKey(e => e.Key);
        layoutNodes.HasOne(e => e.ParentNode)
            .WithMany(p => p.Nodes)
            .HasForeignKey(e => e.ParentNodeId);
        layoutNodes.HasOne(e => e.Component)
            .WithOne(p => p.ParentNode)
            .HasForeignKey<PageComponentSettingsBase>(p => p.ParentNodeId)
            .IsRequired(false);

        layoutNodes.Ignore(e => e.NodeWidths)
            .Ignore(e => e.EditorKey);

        var userScopes = builder.Entity<UserScope>();
        userScopes.HasKey(e => e.Key);

        var userScopeProviderContainers = builder.Entity<UserScopeProviderContainer>();
        userScopeProviderContainers.HasKey(e => e.Key);
        userScopeProviderContainers.HasOne(e => e.ProvidingComponent)
            .WithMany(p => p.ScopeListeners)
            .HasForeignKey(e => e.ProvidingComponentId);
        userScopeProviderContainers.HasOne(e => e.ListeningScope)
            .WithMany(p => p.ScopeProviders)
            .HasForeignKey(e => e.ListeningScopeId);

        var userScopeListenerContainers = builder.Entity<UserScopeListenerContainer>();
        userScopeListenerContainers.HasKey(e => e.Key);
        userScopeListenerContainers.HasOne(e => e.ListeningComponent)
            .WithMany(p => p.ScopeProviders)
            .HasForeignKey(e => e.ListeningComponentId);
        userScopeListenerContainers.HasOne(e => e.ProvidingScope)
            .WithMany(p => p.ScopeListeners)
            .HasForeignKey(e => e.ProvidingScopeId);

        var pageComponentSettingsBase = builder.Entity<PageComponentSettingsBase>();
        pageComponentSettingsBase.HasKey(e => e.Key);
        pageComponentSettingsBase.HasOne(e => e.AuthorizationPolicy)
            .WithMany(p => p.PageComponenetSettings)
            .HasForeignKey(e => e.AuthorizationPolicyKey);
        pageComponentSettingsBase.Navigation(e => e.AuthorizationPolicy)
            .AutoInclude(true);

        var displayComponentSettings = builder.Entity<DisplayComponentSettings>();
        displayComponentSettings.Ignore(e => e.AuthorizedMarkup);

        var editableComponentSettings = builder.Entity<EditableComponentSettings>();
        editableComponentSettings.HasMany(e => e.EditableDisplays)
            .WithMany(e => e.EditableComponentsAllowedEditors);

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

        var textDisplayComponentSettings = builder.Entity<TextDisplayComponentSettings>();
        textDisplayComponentSettings.HasOne(e => e.EditPolicy)
            .WithMany(p => p.TextDisplayComponentSettings)
            .HasForeignKey(e => e.EditPolicyKey);
        textDisplayComponentSettings.Navigation(e => e.EditPolicy)
            .AutoInclude(true);
        textDisplayComponentSettings.Ignore(e => e.Display);
        #endregion

        #region Forms
        var userSelectComponentSettings = builder.Entity<UserSelectComponentSettings>();
        userSelectComponentSettings.HasKey(e => e.Key);
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

        var assignableValueRenderers = builder.Entity<AssignableValueRenderer>();
        assignableValueRenderers.HasKey(e => e.Key);
        assignableValueRenderers.HasMany(e => e.Conversions)
            .WithOne(p => p.Renderer)
            .HasForeignKey(p => p.RendererId);
        assignableValueRenderers.Navigation(e => e.Conversions)
            .AutoInclude(true);

        var assignableValueConversions = builder.Entity<AssignableValueConversion>();
        assignableValueConversions.HasKey(e => e.Key);

        // We give these names manually because EFCORE likes to change the value type
        // for an existing col instead of just making a new col for these values.
        var booleanAssignableConfiguration = builder.Entity<BooleanValueAssignableConfiguration>();
        booleanAssignableConfiguration.Property(e => e.AllowedValues)
            .HasColumnName($"{nameof(BooleanValueAssignableConfiguration)}_{nameof(BooleanValueAssignableConfiguration.AllowedValues)}");

        var booleanAssignableValues = builder.Entity<BooleanAssignableValue>();
        booleanAssignableValues.Property(e => e.SetValue)
            .HasColumnName($"{nameof(BooleanAssignableValue)}_{nameof(BooleanAssignableValue.SetValue)}");
        #endregion

        #region User
        var dataCoreUser = builder.Entity<DataCoreUser>();
        dataCoreUser.HasMany(e => e.RosterSlots)
            .WithOne(e => e.OccupiedBy)
            .HasForeignKey(e => e.OccupiedById);
        dataCoreUser.HasMany(e => e.KeyBindings)
            .WithOne(e => e.DataCoreUser)
            .HasForeignKey(e => e.DataCoreUserId);
        dataCoreUser.Navigation(e => e.AssignableValues)
            .AutoInclude();
            
        var dataCoreUserProperty = builder.Entity<DataCoreUserProperty>();
        dataCoreUserProperty.HasKey(e => e.Key);

        var userKeybindings = builder.Entity<UserKeybinding>();
        userKeybindings.HasKey(e => e.KeyPressed);
        #endregion

        #region Account Link
        var accountLinkSettings = builder.Entity<AccountSettings>();
        accountLinkSettings.HasKey(e => e.Key);
        #endregion

        #region Nav Modules

        var navModules = builder.Entity<NavModule>();
        navModules.HasKey(e => e.Key);
        navModules.HasMany(e => e.SubModules)
            .WithOne(e => e.Parent)
            .HasForeignKey(e => e.ParentId);

        #endregion

        #region Policy
        var dynamicAuthorizationPolicy = builder.Entity<DynamicAuthorizationPolicy>();
        dynamicAuthorizationPolicy.HasKey(e => e.Key);

        // Authorization
        dynamicAuthorizationPolicy.HasMany(p => p.AuthorizedSlots)
            .WithMany(e => e.DynamicPolicies);
        dynamicAuthorizationPolicy.HasMany(p => p.AuthorizedTrees)
            .WithMany(e => e.DynamicPolicies);
        dynamicAuthorizationPolicy.HasMany(p => p.AuthorizedDisplays)
            .WithMany(e => e.DynamicPolicies);
        dynamicAuthorizationPolicy.HasMany(p => p.AuthorizedUsers)
            .WithMany(e => e.DynamicPolicies);

        // Admin
        dynamicAuthorizationPolicy.HasOne(e => e.AdministratorPolicy)
            .WithMany(p => p.WebsitePolciies)
            .HasForeignKey(e => e.AdministratorPolicyKey);

        // Parents
        dynamicAuthorizationPolicy.HasMany(e => e.Parents)
            .WithMany(e => e.Children);

        // Auto include
        dynamicAuthorizationPolicy.Navigation(e => e.AuthorizedSlots)
            .AutoInclude(true);
        dynamicAuthorizationPolicy.Navigation(e => e.AuthorizedTrees)
            .AutoInclude(true);
        dynamicAuthorizationPolicy.Navigation(e => e.AuthorizedDisplays)
            .AutoInclude(true);
        dynamicAuthorizationPolicy.Navigation(e => e.AuthorizedUsers)
            .AutoInclude(true);

        // Ignore
        dynamicAuthorizationPolicy.Ignore(e => e.ValidRosterSlots)
            .Ignore(e => e.ValidUsers);
        #endregion

    }
}