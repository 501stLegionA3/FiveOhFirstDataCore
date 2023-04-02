using FiveOhFirstDataCore.Data.Account;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class WardenSectionData : IEnumerable<Trooper>
    {
        public Trooper SectionLeader { get; set; }
        public Trooper Charlie { get; set; }
        public Trooper Delta { get; set; }
        public Trooper Echo { get; set; }
        public Trooper this[int index]
        {
            get =>
                index switch
                {
                    0 => SectionLeader,
                    1 => Charlie,
                    2 => Delta,
                    3 => Echo,
                    _ => throw new IndexOutOfRangeException(),
                };
            set
            {
                switch(index)
                {
                    case 0:
                        SectionLeader = value;
                        break;
                    case 1:
                        Charlie = value;
                        break;
                    case 2:
                        Delta = value;
                        break;
                    case 3:
                        Echo = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public static int Length = 4;

        public void Assign(Trooper t)
        {
            switch (t.Flight)
            {
                case Flight.Bravo:
                    SectionLeader = t;
                    break;
                case Flight.Charlie:
                    Charlie = t;
                    break;
                case Flight.Delta:
                    Delta = t;
                    break;
                case Flight.Echo:
                    Echo = t;
                    break;
            }
        }

        public IEnumerator<Trooper> GetEnumerator()
        {
            for( int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
