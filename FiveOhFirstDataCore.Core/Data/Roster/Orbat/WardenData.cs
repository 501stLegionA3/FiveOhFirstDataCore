using FiveOhFirstDataCore.Core.Account;
using System.Collections.Generic;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class WardenData : IAssignable<Trooper>
    {
        public Trooper Master { get; set; }
        public Trooper Chief { get; set; }
        public Trooper[] Wardens { get; set; } = new Trooper[9];

        public void Assign(Trooper t)
        {
            switch (t.Role)
            {
                case Role.MasterWarden:
                    Master = t;
                    break;
                case Role.CheifWarden:
                    Chief = t;
                    break;
                case Role.Warden:
                    for (int i = 0; i < Wardens.Length; i++)
                        if (Wardens[i] is null)
                            Wardens[i] = t;
                    break;
            }
        }
    }
}
