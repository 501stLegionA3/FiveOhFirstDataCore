using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class SquadData : IAssignable<Trooper>
    {
        public Trooper Lead { get; set; }
        public Trooper RT { get; set; }
        public TeamData[] Teams { get; set; } = new TeamData[] { new(), new() };
        public Trooper ARC { get; set; }
        public List<Trooper> AdditionalTroopers { get; set; } = new();

        public void Assign(Trooper t)
        {
            switch (t.Role)
            {
                case Role.RTO:
                    if (RT is null)
                        RT = t;
                    else
                        AdditionalTroopers.Add(t);
                    break;
                case Role.ARC:
                    if (ARC is null)
                        ARC = t;
                    else
                        AdditionalTroopers.Add(t);
                    break;
                default:
                    switch (t.Team)
                    {
                        case null:
                            switch (t.Role)
                            {
                                case Role.Lead:
                                    Lead = t;
                                    break;
                                default:
                                    AdditionalTroopers.Add(t);
                                    break;
                            }
                            break;
                        case Team.Alpha:
                            Teams[0].Assign(t);
                            break;
                        case Team.Bravo:
                            Teams[1].Assign(t);
                            break;
                    }
                    break;
            }
        }
    }
}
