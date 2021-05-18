using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FiveOhFirstDataCore.Core.Account
{
    public class Trooper : IdentityUser<int>
    {
        public string NickName { get; set; } = "";

        public TrooperRank Rank { get; set; }
        public RTORank? RTORank { get; set; }
        public MedicRank? MedicRank { get; set; }
        public PilotRank? PilotRank { get; set; }
        public WarrantRank? WarrantRank { get; set; }
        public WardenRank? WardenRank { get; set; }

        public Slot Slot { get; set; }
        public Role Role { get; set; }
        public Team? Team { get; set; }
        public Flight? Flight { get; set; }

        public CShop CShops { get; set; }
        public Qualification Qualifications { get; set; }

        public DateTime LastPromotion { get; set; }
        public DateTime StartOfService { get; set; }

        public string? InitalTraining { get; set; }
        public string? UTC { get; set; }

        public string Notes { get; set; } = "";

        public string? DiscordId { get; set; }
        public string? SteamLink { get; set; }
        public string? AccessCode { get; set; }

        public List<Guid> NotificationItems { get; set; } = new();

        public RecruitStatus RecruitStatus { get; set; }
        public List<TrooperFlag> Flags { get; set; } = new();
        public List<TrooperFlag> CreatedFlags { get; set; } = new();

        #region Logging
        /// <summary>
        /// Rank changes applied to this account.
        /// </summary>
        public List<RankChange> RankChanges { get; set; } = new();
        /// <summary>
        /// Rank changes submitted by this account for other accounts.
        /// </summary>
        public List<RankChange> SubmittedRankChanges { get; set; } = new();
        public List<SlotChange> SlotChanges { get; set; } = new();
        public List<SlotChange> ApprovedSlotChanges { get; set; } = new();
        public List<CShopChange> CShopChanges { get; set; } = new();
        public List<CShopChange> SubmittedCShopChanges { get; set; } = new();
        public List<QualificationChange> QualificationChanges { get; set; } = new();
        public List<QualificationChange> SubmittedQualificationChanges { get; set; } = new();
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

        public string GetRoleName()
        {
            if (Slot == Slot.InactiveReserve)
                return "Inactive Reserves";
            else if (Slot == Slot.Archived)
                return "Archived";
            else if (Slot < Slot.AvalancheCompany)
            {
                return $"Battalion {Role.AsFull()}";
            }
            else if (Slot >= Slot.AvalancheCompany && Slot < Slot.AcklayCompany
                || Slot >= Slot.ZetaCompany)
            {
                var tag = (int)Slot / 10 % 10;
                if(tag == 0)
                {
                    return $"Company {Slot.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if(tag == 0)
                    {
                        return $"Platoon {Slot.AsFull()}";
                    }
                    else
                    {
                        if(Role == Role.Lead)
                        {
                            if(Team is null)
                            {
                                return $"Squad {Role.AsFull()}";
                            }
                            else
                            {
                                return $"Taem {Role.AsFull()}";
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
                    return $"Company {Slot.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Airborne {Slot.AsFull()}";
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
                                return $"Taem {Role.AsFull()}";
                            }
                        }
                        else if (Role == Role.RTO)
                        {
                            return $"Squad {Role.AsFull()}";
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
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Company {Slot.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Platoon {Slot.AsFull()}";
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
                                return $"Taem {Role.AsFull()}";
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
            }
            else if (Slot >= Slot.Razor && Slot < Slot.ZetaCompany)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Squadron {Slot.AsFull()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Flight {Slot.AsFull()}";
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
    }
}
