using FiveOhFirstDataCore.Data.Account.Detail;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Message;
using FiveOhFirstDataCore.Data.Structures.Notice;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures.Notification;
using FiveOhFirstDataCore.Data.Structures.Updates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;


namespace FiveOhFirstDataCore.Data.Account
{
    public class Trooper : IdentityUser<int>
    {
        public string NickName { get; set; } = "";
        public int BirthNumber { get; set; }
        public byte[]? PFP {  get; set; }

        #region Ranks
        public TrooperRank? Rank { get; set; }
        public RTORank? RTORank { get; set; }
        public MedicRank? MedicRank { get; set; }
        public PilotRank? PilotRank { get; set; }
        public WarrantRank? WarrantRank { get; set; }
        public WardenRank? WardenRank { get; set; }
        #endregion
        #region Slots
        public Slot Slot { get; set; }
        public Role Role { get; set; }
        public Team? Team { get; set; }
        public Flight? Flight { get; set; }
        #endregion
        #region C-Shops
        public CShop CShops { get; set; }
        #endregion
        #region Quals
        public Qualification Qualifications { get; set; }
        #endregion

        #region Time Management
        public DateTime LastPromotion { get; set; }
        public DateTime StartOfService { get; set; }
        public DateTime LastBilletChange { get; set; }
        public DateTime GraduatedBCTOn { get; set; }
        public DateTime GraduatedUTCOn { get; set; }
        public DateTime RealBirthday { get; set; }
        public Boolean ShowBirthday { get; set; }

        public DateTime? BilletedCShopLeadership { get; set; }
        public bool IsCShopCommand { get; set; }

        #endregion
        #region Notes
        public string? InitialTraining { get; set; }
        public string? UTC { get; set; }

        public string Notes { get; set; } = "";
        #endregion
        #region MP
        public bool MilitaryPolice { get; set; } = false;
        public List<TrooperReport> FiledReports { get; set; } = new();
        #endregion

        public string? DiscordId { get; set; }
        public string? SteamLink { get; set; }
        public string? AccessCode { get; set; }

        public List<Guid> NotificationItems { get; set; } = new();

        public RecruitStatus RecruitStatus { get; set; }
        public List<TrooperFlag> Flags { get; set; } = new();
        public List<TrooperFlag> CreatedFlags { get; set; } = new();

        public List<Promotion> PendingPromotions { get; set; } = new();
        public List<Promotion> RequestedPromotions { get; set; } = new();
        public List<Promotion> ApprovedPendingPromotions { get; set; } = new();

        public List<TrooperDescription> Descriptions { get; set; } = new();
        public List<TrooperDescription> CreatedDescriptions { get; set; } = new();

        #region Logging
        /// <summary>
        /// Rank changes applied to this account.
        /// </summary>
        public List<RankUpdate> RankChanges { get; set; } = new();
        /// <summary>
        /// Rank changes submitted by this account for other accounts.
        /// </summary>
        public List<RankUpdate> SubmittedRankUpdates { get; set; } = new();
        public List<RankUpdate> ApprovedRankUpdates { get; set; } = new();
        public List<RankUpdate> DeniedRankUpdates { get; set; } = new();
        public List<SlotUpdate> SlotUpdates { get; set; } = new();
        public List<SlotUpdate> ApprovedSlotUpdates { get; set; } = new();
        public List<CShopUpdate> CShopUpdates { get; set; } = new();
        public List<CShopUpdate> SubmittedCShopUpdates { get; set; } = new();
        public List<QualificationUpdate> QualificationUpdates { get; set; } = new();
        public List<QualificationUpdate> SubmittedQualificationUpdates { get; set; } = new();
        public List<RecruitmentUpdate> Recruitments { get; set; } = new();
        public List<NickNameUpdate> NickNameUpdates { get; set; } = new();
        public List<NickNameUpdate> ApprovedNickNameUpdates { get; set; } = new();
        public List<ClaimUpdate> ClaimUpdates { get; set; } = new();
        public List<ClaimUpdate> AuthorizedClaimUpdates { get; set; } = new();
        public List<TimeUpdate> TimeUpdates { get; set; } = new();
        public List<TimeUpdate> ApprovedTimeUpdates { get; set; } = new();
        public RecruitmentUpdate RecruitedByData { get; set; }
        #endregion

        #region Disciplinary Actions
        /// <summary>
        /// Holds the DA's filed against this Trooper
        /// </summary>
        public List<DisciplinaryAction> DisciplinaryActions { get; set; } = new();
        /// <summary>
        /// Holds the DA's filed by this Trooper
        /// </summary>
        public List<DisciplinaryAction> FiledDisciplinaryActions { get; set; } = new();
        /// <summary>
        /// Holds the DA's this Trooper has been recorded as witnessed.
        /// </summary>
        public List<DisciplinaryAction> WitnessedDisciplinaryActions { get; set; } = new();
        /// <summary>
        /// Holds the DA's that have been sent to this Trooper for further review. This is not DA's filed against this Trooper.
        /// </summary>
        public List<DisciplinaryAction> DisciplinaryActionInbox { get; set; } = new();
        #endregion

        #region Notifications
        public List<ReportNotificationTracker> TrooperReportTrackers { get; set; } = new();
        #endregion

        public List<TrooperChangeRequestData> TrooperChangeRequests { get; set; } = new();
        public List<TrooperChangeRequestData> FinalizedChangeRequests { get; set; } = new();

        public List<Notice> NoticesWritten { get; set; } = new();
        public List<TrooperMessage> TrooperMessages { get; set; } = new();

        public DateTime LastUpdate { get; set; }
        public bool PermissionsView { get; set; } = false;

        public Trooper ShallowCopy()
            => (Trooper)MemberwiseClone();

        public string GetRankDesignation()
        {
            if (Role == Role.RTO)
            {
                return RTORank?.AsShorthand() ?? "";
            }
            else if (Slot >= Slot.Razor && Slot < Slot.Warden
                || Slot == Slot.RazorReserve)
            {
                return PilotRank?.AsShorthand() ?? WardenRank?.AsShorthand() ?? "";
            }
            else if (Role == Role.Medic)
            {
                return MedicRank?.AsShorthand() ?? "";
            }
            else if (Slot >= Slot.Warden && Slot < Slot.ZetaCompany)
            {
                return WardenRank?.AsShorthand() ?? "";
            }
            else if (WarrantRank is not null)
            {
                return WarrantRank?.AsShorthand() ?? "";
            }
            else
            {
                return Rank?.AsShorthand() ?? "";
            }
        }

        public string GetRankName()
        {
            if (Slot >= Slot.Razor && Slot < Slot.Warden
                || Slot == Slot.RazorReserve)
            {
                return PilotRank?.AsFull() ?? WardenRank?.AsFull() ?? "";
            }
            else if (Slot >= Slot.Warden && Slot < Slot.ZetaCompany)
            {
                return WardenRank?.AsFull() ?? "";
            }
            else if (WarrantRank is not null)
            {
                return WarrantRank?.AsFull() ?? "";
            }
            else
            {
                return Rank?.AsFull() ?? "";
            }
        }

        public string GetRoleName()
        {
            if (Slot >= Slot.InactiveReserve)
                return Slot.AsFull();
            else if (Slot < Slot.AvalancheCompany)
            {
                return $"Battalion {Role.AsFull()}";
            }
            else if (Slot >= Slot.AvalancheCompany && Slot < Slot.AcklayCompany
                || Slot >= Slot.ZetaCompany)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Company {Role.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Platoon {Role.AsFull()}";
                    }
                    else
                    {
                        if (Role == Role.Lead)
                        {
                            if (Team is null)
                            {
                                return $"Squad {Role.AsFull()}";
                            }
                            else
                            {
                                return $"Team {Role.AsFull()}";
                            }
                        }
                        else if (Role == Role.RTO)
                        {
                            return $"Squad {Role.AsFull()}";
                        }
                        else
                        {
                            return Role.AsFull();
                        }
                    }
                }
            }
            else if (Slot >= Slot.AcklayCompany && Slot < Slot.Mynock)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Airborne Company {Role.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Airborne Platoon {Role.AsFull()}";
                    }
                    else
                    {
                        if (Role == Role.Lead)
                        {
                            if (Team is null)
                            {
                                return $"Airborne Squad {Role.AsFull()}";
                            }
                            else
                            {
                                return $"Airborne Team {Role.AsFull()}";
                            }
                        }
                        else if (Role == Role.RTO)
                        {
                            return $"Airborne Squad {Role.AsFull()}";
                        }
                        else
                        {
                            return $"Airborne {Role.AsFull()}";
                        }
                    }
                }
            }
            else if (Slot >= Slot.Mynock && Slot < Slot.Razor)
            {
                var tag = (int)Slot % 10;
                if (tag == 0)
                {
                    return $"Detachement {Role.AsFull()}";
                }
                else
                {
                    if (Role == Role.Lead)
                    {
                        if (Team is null)
                        {
                            return $"Section {Role.AsFull()}";
                        }
                        else
                        {
                            return $"Team {Role.AsFull()}";
                        }
                    }
                    else if (Role == Role.RTO)
                    {
                        return $"Section {Role.AsFull()}";
                    }
                    else
                    {
                        return Role.AsFull();
                    }
                }
            }
            else if (Slot >= Slot.Razor && Slot < Slot.ZetaCompany)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Squadron {Role.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Flight {Role.AsFull()}";
                    }
                    else
                    {
                        return Role.AsFull();
                    }
                }
            }
            else
            {
                return "";
            }
        }

        public bool IsRealBirthday()
        {
            return RealBirthday.IsAnniversary(DateTime.Now);
        }

        public bool IsCloneBirthday()
        {
            return StartOfService.IsAnniversary(DateTime.Now);
        }

        public MarkupString BirthdayCake()
        {
            string name = "";
            if (IsRealBirthday())
            {
                name += " " + "<span class=\"fas fa-birthday-cake\"></span><span class=\"fas fa-birthday-cake\"></span><span class=\"fas fa-birthday-cake\"></span>";
            }
            if (IsCloneBirthday())
            {
                name += " " + "<span class=\"fas fa-birthday-cake\"></span>";
            }
            return new MarkupString(name);
        }

        public MarkupString DisplayRankName()
        {
            string name = "";
            name += GetRankName();
            name += " " + NickName;

            return new MarkupString(name + BirthdayCake().Value);
        }
    }
}
