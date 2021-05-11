using FiveOhFirstDataCore.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;

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

        public RecruitStatus RecruitStatus { get; set; } = new();

        public string GetRoleName()
        {
            if (Slot == Slot.InactiveReserve)
                return "Inactive Reserves";
            else if (Slot == Slot.Archived)
                return "Archived";
            else if (Slot < Slot.AvalancheCompany)
            {
                return $"Battalion {Role.AsName()}";
            }
            else if (Slot >= Slot.AvalancheCompany && Slot < Slot.AcklayCompany
                || Slot >= Slot.ZetaCompany)
            {
                var tag = (int)Slot / 10 % 10;
                if(tag == 0)
                {
                    return $"Company {Slot.AsName()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if(tag == 0)
                    {
                        return $"Platoon {Slot.AsName()}";
                    }
                    else
                    {
                        if(Role == Role.Lead)
                        {
                            if(Team is null)
                            {
                                return $"Squad {Role.AsName()}";
                            }
                            else
                            {
                                return $"Taem {Role.AsName()}";
                            }
                        }
                        else if (Role == Role.RTO)
                        {
                            return $"Squad {Role.AsName()}";
                        }
                        else
                        {
                            return Role.AsName();
                        }
                    }
                }
            }
            else if (Slot >= Slot.AcklayCompany && Slot < Slot.Mynock)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Company {Slot.AsName()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Airborne {Slot.AsName()}";
                    }
                    else
                    {
                        if (Role == Role.Lead)
                        {
                            if (Team is null)
                            {
                                return $"Squad {Role.AsName()}";
                            }
                            else
                            {
                                return $"Taem {Role.AsName()}";
                            }
                        }
                        else if (Role == Role.RTO)
                        {
                            return $"Squad {Role.AsName()}";
                        }
                        else
                        {
                            return $"Airborne {Role.AsName()}";
                        }
                    }
                }
            }
            else if (Slot >= Slot.Mynock && Slot < Slot.Razor)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Company {Slot.AsName()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Platoon {Slot.AsName()}";
                    }
                    else
                    {
                        if (Role == Role.Lead)
                        {
                            if (Team is null)
                            {
                                return $"Section {Role.AsName()}";
                            }
                            else
                            {
                                return $"Taem {Role.AsName()}";
                            }
                        }
                        else if (Role == Role.RTO)
                        {
                            return $"Section {Role.AsName()}";
                        }
                        else
                        {
                            return Role.AsName();
                        }
                    }
                }
            }
            else if (Slot >= Slot.Razor && Slot < Slot.ZetaCompany)
            {
                var tag = (int)Slot / 10 % 10;
                if (tag == 0)
                {
                    return $"Squadron {Slot.AsName()}";
                }
                else
                {
                    tag = (int)Slot % 10;
                    if (tag == 0)
                    {
                        return $"Flight {Slot.AsName()}";
                    }
                    else
                    {
                        return Role.AsName();
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
