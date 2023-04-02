using FiveOhFirstDataCore.Data.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class RazorWingData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper SubCommander { get; set; }
        public Trooper COO { get; set; }
        public Trooper CLO { get; set; }

        public RazorSquadronData[] Squadrons { get; set; } = new RazorSquadronData[] { new(), new() };
        public WardenData WardenData { get; set; } = new();
        public void Assign(Trooper t)
        {
            var val = (int)t.Slot / 10 % 10;
            if (val == 0)
            {
                switch (t.Role)
                {
                    case Role.Commander:
                        Commander = t;
                        break;
                    case Role.SubCommander:
                        SubCommander = t;
                        break;
                    case Role.WCOO:
                        COO = t;
                        break;
                    case Role.WCLO:
                        CLO = t;
                        break;
                }
            }
            else if (val >= 1 && val <= Squadrons.Length)
            {
                Squadrons[val - 1].Assign(t);
            }
            // 531 / 10 = 53 % 10 = 3
            // 530 / 10 = 53 % 10 = 3
            else if (val == ((int)Slot.Warden / 10 % 10))
            {
                WardenData.Assign(t);
            }
        }
    }
}
