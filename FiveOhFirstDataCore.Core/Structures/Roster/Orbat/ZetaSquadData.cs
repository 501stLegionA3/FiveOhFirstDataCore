using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class ZetaSquadData : IAssignable<Trooper>
    {
        public Trooper SquadLeader { get; set; }
        public Trooper Leader { get; set; }
        public Trooper RT { get; set; }
        public Trooper Medic { get; set; }
        public Trooper[] Troopers { get; private set; } = new Trooper[11];

        public void Assign(Trooper item)
        {
            if (item.Team is null)
            {
                SquadLeader = item;
            }
            else
            {
                switch (item.Role)
                {
                    case Role.Lead:
                        Leader = item;
                        break;
                    case Role.RTO:
                        RT = item;
                        break;
                    case Role.Medic:
                        Medic = item;
                        break;
                    default:
                        for (int i = 0; i < Troopers.Length; i++)
                        {
                            if (Troopers[i] is null)
                            {
                                Troopers[i] = item;
                                break;
                            }
                        }
                        break;
                }
            }
        }
    }

    public class ZetaUTCSquadData : IAssignable<Trooper>
    {
        public Trooper SquadLeader { get; set; }
        public Trooper[] Troopers { get; private set; } = new Trooper[10];

        public void Assign(Trooper item)
        {
            if (item.Role == Role.Lead)
            {
                SquadLeader = item;
            }
            else
            {
                for (int i = 0; i < Troopers.Length; i++)
                {
                    if (Troopers[i] is null)
                    {
                        Troopers[i] = item;
                        break;
                    }
                }
            }
        }
    }
}
