using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ProjectDataCore.Data.Account;
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

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region Roster
        var rosterTree = builder.Entity<RosterTree>();
        rosterTree.HasKey(e => e.Key);


        var rosterSlot = builder.Entity<RosterSlot>();
        rosterSlot.HasKey(e => e.Key);

        var rosertDisplaySettings = builder.Entity<RosterDisplaySettings>();
        rosertDisplaySettings.HasKey(e => e.Key);
        #endregion

        #region User
        var dataCoreUser = builder.Entity<DataCoreUser>();
        dataCoreUser.HasMany(e => e.RosterSlots)
            .WithOne(e => e.OccupiedBy)
            .HasForeignKey(e => e.OccupiedById);
            
        #endregion

        base.OnModelCreating(builder);
    }
}
