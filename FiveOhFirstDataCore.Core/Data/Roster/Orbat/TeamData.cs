using FiveOhFirstDataCore.Core.Account;

using System.Collections.Generic;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class TeamData : IAssignable<Trooper>
    {
        public Trooper Lead { get; set; }
        public List<Trooper> Troopers { get; set; } = new();
        public Trooper Medic { get; set; }

        public void Assign(Trooper t)
        {
            switch (t.Role)
            {
                case Role.Lead:
                    Lead = t;
                    break;
                case Role.Trooper:
                    Troopers.Add(t);
                    break;
                case Role.Medic:
                    Medic = t;
                    break;
            }
        }
    }
}
