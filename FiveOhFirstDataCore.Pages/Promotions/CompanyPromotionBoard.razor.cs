using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Roster;

using FiveOhFirstDataCore.Data.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Promotions;

public partial class CompanyPromotionBoard
{
    [Parameter]
    public CompanyData? Company { get; set; }

    [Parameter]
    public ZetaCompanyData? Zeta { get; set; }
    [Parameter]
    public int Slot { get; set; } = -1;

    [CascadingParameter]
    public Trooper CurrentUser { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthStateTask { get; set; }

    public List<Trooper>? Troopers { get; set; } = null;

    private bool CanPromote { get; set; } = false;

    private HashSet<int> CanPromoteValues { get; set; } = new();

    private PromotionType PromoType = PromotionType.Default;


    private void BuildTrooperList()
    {
        Troopers = new();
        if (Company is not null)
        {
            if (Company.RT is not null)
                Troopers.Add(Company.RT);
            if (Company.ARC is not null)
                Troopers.Add(Company.ARC);
            if (Company.Medic is not null)
                Troopers.Add(Company.Medic);
            if (Company.XO is not null)
                Troopers.Add(Company.XO);
            if (Company.NCOIC is not null)
                Troopers.Add(Company.NCOIC);

            foreach (var platoon in Company.Platoons)
            {
                if (platoon.Commander is not null)
                    Troopers.Add(platoon.Commander);
                if (platoon.SergeantMajor is not null)
                    Troopers.Add(platoon.SergeantMajor);
                if (platoon.ARC is not null)
                    Troopers.Add(platoon.ARC);
                if (platoon.Medic is not null)
                    Troopers.Add(platoon.Medic);
                if (platoon.RT is not null)
                    Troopers.Add(platoon.RT);

                foreach (var squad in platoon.Squads)
                {
                    if (squad.Lead is not null)
                        Troopers.Add(squad.Lead);
                    if (squad.RT is not null)
                        Troopers.Add(squad.RT);
                    if (squad.ARC is not null)
                        Troopers.Add(squad.ARC);

                    foreach (var t in squad.Teams)
                    {
                        if (t.Lead is not null)
                            Troopers.Add(t.Lead);
                        if (t.Medic is not null)
                            Troopers.Add(t.Medic);

                        Troopers.AddRange(t.Troopers.Where(x => x != null));
                    }

                    Troopers.AddRange(squad.AdditionalTroopers);
                }
            }
        }
        else if (Zeta is not null)
        {
            if (Zeta.Commander is not null)
                Troopers.Add(Zeta.Commander);
            if (Zeta.XO is not null)
                Troopers.Add(Zeta.XO);
            if (Zeta.NCOIC is not null)
                Troopers.Add(Zeta.NCOIC);
            if (Zeta.Adjutant is not null)
                Troopers.Add(Zeta.Adjutant);

            foreach (var section in Zeta.Sections)
            {
                if (section.Commander is not null)
                    Troopers.Add(section.Commander);
                if (section.Subordinate is not null)
                    Troopers.Add(section.Subordinate);

                foreach (var s in section.Squads)
                {
                    if (s.SquadLeader is not null)
                        Troopers.Add(s.SquadLeader);
                    if (s.Leader is not null)
                        Troopers.Add(s.Leader);
                    if (s.RT is not null)
                        Troopers.Add(s.RT);
                    if (s.Medic is not null)
                        Troopers.Add(s.Medic);
                    foreach (var t in s.Troopers)
                        if (t is not null)
                            Troopers.Add(t);
                }
            }
        }

        switch (PromoType)
        {
            case PromotionType.Default:
                CanPromoteValues = Troopers.ToHashSet(x => x.Id);
                break;
            case PromotionType.RTO:
                CanPromoteValues = Troopers.AsQueryable()
                    .Where(x => x.Role == Role.RTO)
                    .ToHashSet(x => x.Id);
                break;
            case PromotionType.Medic:
                CanPromoteValues = Troopers.AsQueryable()
                    .Where(x => x.Role == Role.Medic)
                    .ToHashSet(x => x.Id);
                break;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var user = (await AuthStateTask).User;

        bool manager = (await _auth.AuthorizeAsync(user, "RequireManager")).Succeeded
            || user.HasClaim("Promotion.Company", Slot.ToString());

        if (Company is not null)
        {
            CanPromote = manager
                || (Company?.Commander?.Id ?? 0) == CurrentUser?.Id
                || (Company?.XO?.Id ?? 0) == CurrentUser?.Id
                || (Company?.NCOIC?.Id ?? 0) == CurrentUser?.Id
                || (Company?.Adjutant?.Id ?? 0) == CurrentUser?.Id;

            if (!CanPromote)
            {
                if ((Company?.RT?.Id ?? 0) == CurrentUser?.Id)
                {
                    CanPromote = true;
                    PromoType = PromotionType.RTO;
                }
                else if ((Company?.Medic?.Id ?? 0) == CurrentUser?.Id)
                {
                    CanPromote = true;
                    PromoType = PromotionType.Medic;
                }
            }
        }
        else if (Zeta is not null)
        {
            CanPromote = manager
                || (Zeta?.Commander?.Id ?? 0) == CurrentUser?.Id
                || (Zeta?.XO?.Id ?? 0) == CurrentUser?.Id
                || (Zeta?.NCOIC?.Id ?? 0) == CurrentUser?.Id
                || (Zeta?.Adjutant?.Id ?? 0) == CurrentUser?.Id;
        }

        BuildTrooperList();
    }
}
