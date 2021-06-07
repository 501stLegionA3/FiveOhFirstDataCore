using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Components;
using FiveOhFirstDataCore.Core.Data.Notice;
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

        public DbSet<ClaimUpdateData> ClaimUpdateData { get; internal set; }

        public DbSet<DisciplinaryAction> DisciplinaryActions { get; internal set; }
        public DbSet<TrooperFlag> TrooperFlags { get; internal set; }
        public DbSet<NoticeBoardData> NoticeBoards { get; internal set; }
        public DbSet<Notice> Notices { get; internal set; }

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

            var rankChange = builder.Entity<RankUpdate>();
            rankChange.HasKey(e => e.ChangeId);
            rankChange.HasOne(e => e.ChangedFor)
                .WithMany(p => p.RankChanges)
                .HasForeignKey(e => e.ChangedForId);
            rankChange.HasOne(e => e.ChangedBy)
                .WithMany(p => p.SubmittedRankUpdates)
                .HasForeignKey(e => e.ChangedById);

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
                .HasForeignKey(e => e.ChangedById);

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

            var flags = builder.Entity<TrooperFlag>();
            flags.HasKey(e => e.FlagId);
            flags.HasOne(e => e.FlagFor)
                .WithMany(p => p.Flags)
                .HasForeignKey(e => e.FlagForId);
            flags.HasOne(e => e.Author)
                .WithMany(p => p.CreatedFlags)
                .HasForeignKey(e => e.AuthorId);

            var recruit = builder.Entity<RecruitmentUpdate>();
            recruit.HasKey(e => e.ChangeId);
            recruit.HasOne(e => e.ChangedFor)
                .WithOne(p => p.RecruitedByData)
                .HasForeignKey<RecruitmentUpdate>(e => e.ChangedForId);
            recruit.HasOne(e => e.RecruitedBy)
                .WithMany(p => p.Recruitments)
                .HasForeignKey(e => e.RecruitedById);

            var nickname = builder.Entity<NickNameUpdate>();
            nickname.HasKey(e => e.ChangeId);
            nickname.HasOne(e => e.ChangedFor)
                .WithMany(p => p.NickNameUpdates)
                .HasForeignKey(e => e.ChangedForId);
            nickname.HasOne(e => e.ApprovedBy)
                .WithMany(p => p.ApprovedNickNameUpdates)
                .HasForeignKey(e => e.ApprovedById);

            var claims = builder.Entity<ClaimUpdate>();
            claims.HasKey(e => e.ChangeId);
            claims.HasOne(e => e.ChangedFor)
                .WithMany(p => p.ClaimUpdates)
                .HasForeignKey(e => e.ChangedForId);
            claims.HasOne(e => e.ChangedBy)
                .WithMany(p => p.AuthorizedClaimUpdates)
                .HasForeignKey(e => e.ChangedById);


            var claimData = builder.Entity<ClaimUpdateData>();
            claimData.HasKey(e => e.UpdateKey);

            var noticeBoard = builder.Entity<NoticeBoardData>();
            noticeBoard.HasKey(e => e.Loaction);

            var notices = builder.Entity<Notice>();
            notices.HasKey(e => e.NoticeId);
            notices.HasOne(e => e.NoticeBoard)
                .WithMany(p => p.Notices)
                .HasForeignKey(e => e.NoticeBoardName);
            notices.HasOne(e => e.Author)
                .WithMany(p => p.NoticesWriten)
                .HasForeignKey(e => e.AuthorId);
            notices.Ignore(e => e.Display);
        }
    }
}
