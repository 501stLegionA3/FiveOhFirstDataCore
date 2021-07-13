using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Components;
using FiveOhFirstDataCore.Core.Data.Notice;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiveOhFirstDataCore.Core.Database
{
    public class ApplicationDbContext : IdentityDbContext<Trooper, TrooperRole, int>
    {
        public DbSet<RecruitStatus> RecruitStatuses { get; internal set; }

        public DbSet<RankUpdate> RankUpdates { get; internal set; }
        public DbSet<SlotUpdate> SlotUpdates { get; internal set; }
        public DbSet<CShopUpdate> CShopUpdates { get; internal set; }
        public DbSet<QualificationUpdate> QualificationUpdates { get; internal set; }
        public DbSet<RecruitmentUpdate> RecruitmentUpdates { get; internal set; }
        public DbSet<ClaimUpdate> ClaimUpdates { get; internal set; }
        public DbSet<TimeUpdate> TimeUpdates { get; internal set; }

        public DbSet<ClaimUpdateData> ClaimUpdateData { get; internal set; }

        public DbSet<DisciplinaryAction> DisciplinaryActions { get; internal set; }
        public DbSet<TrooperFlag> TrooperFlags { get; internal set; }
        public DbSet<NoticeBoardData> NoticeBoards { get; internal set; }
        public DbSet<Notice> Notices { get; internal set; }

        public DbSet<Promotion> Promotions { get; internal set; }

        #region Website Settings
        public DbSet<PromotionRequirements> PromotionRequirements { get; internal set; }
        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Account Setup

            var recruitStatus = builder.Entity<RecruitStatus>();
            recruitStatus.HasKey(e => e.RecruitStatusId);
            recruitStatus.HasOne(e => e.Trooper)
                .WithOne(p => p.RecruitStatus)
                .HasForeignKey<RecruitStatus>(e => e.TrooperId);

            #endregion

            #region Website Settings
            var promoReq = builder.Entity<PromotionRequirements>();
            promoReq.HasKey(p => p.RequirementsFor);
            #endregion

            #region Promotions and Transfers

            var promotion = builder.Entity<Promotion>();
            promotion.HasKey(p => p.Id);
            promotion.HasOne(p => p.PromotionFor)
                .WithMany(e => e.PendingPromotions)
                .HasForeignKey(p => p.PromotionForId);
            promotion.HasOne(p => p.RequestedBy)
                .WithMany(e => e.RequestedPromotions)
                .HasForeignKey(p => p.RequestedById)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            promotion.Property(p => p.RequestedById)
                .IsRequired(false);
            promotion.HasMany(p => p.ApprovedBy)
                .WithMany(e => e.ApprovedPendingPromotions);

            #endregion

            #region Logging

            var rankChange = builder.Entity<RankUpdate>();
            rankChange.HasKey(e => e.ChangeId);
            rankChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.RankChanges)
                .HasForeignKey(e => e.ChangedForId);
            rankChange.HasOne(e => e.RequestedBy)
                .WithMany(p => p.SubmittedRankUpdates)
                .HasForeignKey(e => e.RequestedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            rankChange.Property(p => p.RequestedById)
                .IsRequired(false);
            rankChange.HasMany(e => e.ApprovedBy)
                .WithMany(p => p.ApprovedRankUpdates);
            rankChange.HasOne(e => e.DeniedBy)
                .WithMany(p => p.DeniedRankUpdates)
                .HasForeignKey(e => e.DeniedById)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            rankChange.Property(p => p.RequestedById)
                .IsRequired(false);

            var slotChange = builder.Entity<SlotUpdate>();
            slotChange.HasKey(e => e.ChangeId);
            slotChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.SlotUpdates)
                .HasForeignKey(e => e.ChangedForId);
            slotChange.HasMany(e => e.ApprovedBy)
                .WithMany(p => p.ApprovedSlotUpdates);

            var shopChange = builder.Entity<CShopUpdate>();
            shopChange.HasKey(e => e.ChangeId);
            shopChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.CShopUpdates)
                .HasForeignKey(e => e.ChangedForId);
            shopChange.HasOne(e => e.ChangedBy)
                .WithMany(p => p.SubmittedCShopUpdates)
                .HasForeignKey(e => e.ChangedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            shopChange.Property(p => p.ChangedById)
                .IsRequired(false);

            var qualChange = builder.Entity<QualificationUpdate>();
            qualChange.HasKey(e => e.ChangeId);
            qualChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.QualificationUpdates)
                .HasForeignKey(e => e.ChangedForId);
            qualChange.HasMany(e => e.Instructors)
                .WithMany(p => p.SubmittedQualificationUpdates);

            var da = builder.Entity<DisciplinaryAction>();
            da.HasKey(e => e.DAID);
            da.HasOne(e => e.FiledBy)
                .WithMany(p => p.FiledDisciplinaryActions)
                .HasForeignKey(e => e.FiledById);
            da.HasOne(e => e.FiledTo)
                .WithMany(p => p.DisciplinaryActionInbox)
                .HasForeignKey(e => e.FiledToId);
            da.HasOne(e => e.FiledAgainst)
                .WithMany(p => p.DisciplinaryActions)
                .HasForeignKey(e => e.FiledAgainstId);
            da.HasMany(e => e.Witnesses)
                .WithMany(p => p.WitnessedDisciplinaryActions);

            var recruit = builder.Entity<RecruitmentUpdate>();
            recruit.HasKey(e => e.ChangeId);
            recruit.HasOne(e => e.ChangedFor)
                .WithOne(p => p.RecruitedByData)
                .HasForeignKey<RecruitmentUpdate>(e => e.ChangedForId);
            recruit.HasOne(e => e.RecruitedBy)
                .WithMany(p => p.Recruitments)
                .HasForeignKey(e => e.RecruitedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            recruit.Property(p => p.RecruitedById)
                .IsRequired(false);

            var nickname = builder.Entity<NickNameUpdate>();
            nickname.HasKey(e => e.ChangeId);
            nickname.HasOne(e => e.ChangedFor)
                .WithMany(p => p.NickNameUpdates)
                .HasForeignKey(e => e.ChangedForId);
            nickname.HasOne(e => e.ApprovedBy)
                .WithMany(p => p.ApprovedNickNameUpdates)
                .HasForeignKey(e => e.ApprovedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            nickname.Property(p => p.ApprovedById)
                .IsRequired(false);

            var claims = builder.Entity<ClaimUpdate>();
            claims.HasKey(e => e.ChangeId);
            claims.HasOne(e => e.ChangedFor)
                .WithMany(p => p.ClaimUpdates)
                .HasForeignKey(e => e.ChangedForId);
            claims.HasOne(e => e.ChangedBy)
                .WithMany(p => p.AuthorizedClaimUpdates)
                .HasForeignKey(e => e.ChangedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            claims.Property(p => p.ChangedById)
                .IsRequired(false);

            var times = builder.Entity<TimeUpdate>();
            times.HasKey(e => e.ChangeId);
            times.HasOne(e => e.ChangedFor)
                .WithMany(p => p.TimeUpdates)
                .HasForeignKey(e => e.ChangedForId);
            times.HasOne(e => e.ChangedBy)
                .WithMany(p => p.ApprovedTimeUpdates)
                .HasForeignKey(e => e.ChangedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            times.Property(p => p.ChangedById)
                .IsRequired(false);

            #endregion Logging

            #region Boards

            var flags = builder.Entity<TrooperFlag>();
            flags.HasKey(e => e.FlagId);
            flags.HasOne(e => e.FlagFor)
                .WithMany(p => p.Flags)
                .HasForeignKey(e => e.FlagForId);
            flags.HasOne(e => e.Author)
                .WithMany(p => p.CreatedFlags)
                .HasForeignKey(e => e.AuthorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            flags.Property(p => p.AuthorId)
                .IsRequired(false);

            var noticeBoard = builder.Entity<NoticeBoardData>();
            noticeBoard.HasKey(e => e.Location);

            var notices = builder.Entity<Notice>();
            notices.HasKey(e => e.NoticeId);
            notices.HasOne(e => e.NoticeBoard)
                .WithMany(p => p.Notices)
                .HasForeignKey(e => e.NoticeBoardName);
            notices.HasOne(e => e.Author)
                .WithMany(p => p.NoticesWritten)
                .HasForeignKey(e => e.AuthorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            notices.Property(p => p.AuthorId)
                .IsRequired(false);
            notices.Ignore(e => e.Display);

            #endregion Boards

            var claimData = builder.Entity<ClaimUpdateData>();
            claimData.HasKey(e => e.UpdateKey);
        }
    }
}
