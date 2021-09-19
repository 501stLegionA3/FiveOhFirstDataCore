using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class ZetaSectionData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper Subordinate { get; set; }
        public ZetaSquadData[] Squads { get; private set; } = Array.Empty<ZetaSquadData>();

        public ZetaSectionData(int squads)
        {
            Squads = new ZetaSquadData[squads];
            for (int i = 0; i < Squads.Length; i++)
                Squads[i] = new();
        }

        public void Assign(Trooper item)
        {
            var val = (int)item.Slot % 10;
            if (val == 0)
            {
                switch (item.Role)
                {
                    case Role.Commander:
                        Commander = item;
                        break;
                    case Role.Subordinate:
                        Subordinate = item;
                        break;
                }
            }
            else
            {
                Squads[val - 1].Assign(item);
            }
        }
    }

    public class ZetaUTCSectionData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper Subordinate { get; set; }
        public ZetaUTCSquadData[] Squads { get; private set; } = Array.Empty<ZetaUTCSquadData>();

        public ZetaUTCSectionData(int squads)
        {
            Squads = new ZetaUTCSquadData[squads];
            for (int i = 0; i < Squads.Length; i++)
                Squads[i] = new();
        }

        public void Assign(Trooper item)
        {
            var val = (int)item.Slot % 10;
            if (val == 0)
            {
                switch (item.Role)
                {
                    case Role.Commander:
                        Commander = item;
                        break;
                    case Role.Subordinate:
                        Subordinate = item;
                        break;
                }
            }
            else
            {
                Squads[val - 1].Assign(item);
            }
        }
    }
}
