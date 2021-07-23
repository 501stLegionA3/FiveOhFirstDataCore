using FiveOhFirstDataCore.Core.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class WardenSection : IAssignable<Trooper>
    {
        public Trooper SectionLead { get; set; }
        public Trooper WardenTwo { get; set; }
        public Trooper WardenThree { get; set; }

        public void Assign(Trooper item)
        {
            if (item.Flight == Flight.Alpha)
                SectionLead = item;
            else if (item.Flight == Flight.Bravo)
                WardenTwo = item;
            else if(item.Flight == Flight.Charlie)
                WardenThree = item;
        }
    }
}
