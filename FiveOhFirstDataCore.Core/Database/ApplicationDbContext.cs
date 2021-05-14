using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiveOhFirstDataCore.Core.Database
{
    public class ApplicationDbContext : IdentityDbContext<Trooper, TrooperRole, int>
    {
        public DbSet<RecruitStatus> RecruitStatuses { get; internal set; }
        public DbSet<RankChange> RankChanges { get; internal set; }
        public DbSet<SlotChange> SlotChanges { get; internal set; }
        public DbSet<CShopChange> CShopChanges { get; internal set; }
        public DbSet<QualificationChange> QualificationChanges { get; internal set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var recruitStatus = builder.Entity<RecruitStatus>();
            recruitStatus.HasKey(e => e.RecruitStatusKey);
            recruitStatus.HasOne(e => e.Trooper)
                .WithOne(p => p.RecruitStatus)
                .HasForeignKey<RecruitStatus>(e => e.TrooperId);

            var rankChange = builder.Entity<RankChange>();
            rankChange.HasKey(e => e.ChangeId);
            rankChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.RankChanges)
                .HasForeignKey(e => e.ChangedForId);
            rankChange.HasOne(e => e.ChangedBy)
                .WithMany(p => p.SubmittedRankChanges)
                .HasForeignKey(e => e.ChangedById);

            var slotChange = builder.Entity<SlotChange>();
            slotChange.HasKey(e => e.ChangeId);
            slotChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.SlotChanges)
                .HasForeignKey(e => e.ChangedForId);
            slotChange.HasMany(e => e.ApprovedBy)
                .WithMany(p => p.ApprovedSlotChanges);

            var shopChange = builder.Entity<CShopChange>();
            shopChange.HasKey(e => e.ChangeId);
            shopChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.CShopChanges)
                .HasForeignKey(e => e.ChangedForId);
            shopChange.HasOne(e => e.ChangedBy)
                .WithMany(p => p.SubmittedCShopChanges)
                .HasForeignKey(e => e.ChangedById);

            var qualChange = builder.Entity<QualificationChange>();
            qualChange.HasKey(e => e.ChangeId);
            qualChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.QualificationChanges)
                .HasForeignKey(e => e.ChangedForId);
            qualChange.HasMany(e => e.Instructors)
                .WithMany(p => p.SubmittedQualificationChanges);
        }
    }
}
