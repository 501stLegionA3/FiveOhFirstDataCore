using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class MynockSectionData : IAssignable<Trooper>
    {
        public Trooper Lead { get; set; }
        public Trooper RT { get; set; }
        public TeamData[] Teams { get; set; } = new TeamData[] { new(), new() };

        public void Assign(Trooper t)
        {
            switch (t.Team)
            {
                case null:
                    switch (t.Role)
                    {
                        case Role.Lead:
                            Lead = t;
                            break;
                        case Role.RTO:
                            RT = t;
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
        }
    }
}
