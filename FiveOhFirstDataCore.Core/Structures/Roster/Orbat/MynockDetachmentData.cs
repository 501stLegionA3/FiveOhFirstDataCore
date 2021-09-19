using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class MynockDetachmentData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper NCOIC { get; set; }
        public Trooper Medic { get; set; }
        public Trooper RT { get; set; }
        public MynockSectionData[] Sections { get; set; } = new MynockSectionData[] { new(), new(), new() };

        public void Assign(Trooper t)
        {
            var val = (int)t.Slot % 10;
            if (val == 0)
            {
                switch (t.Role)
                {
                    case Role.Commander:
                        Commander = t;
                        break;
                    case Role.NCOIC:
                        NCOIC = t;
                        break;
                    case Role.Medic:
                        Medic = t;
                        break;
                    case Role.RTO:
                        RT = t;
                        break;
                }
            }
            else if (val >= 1 && val <= Sections.Length)
            {
                Sections[val - 1].Assign(t);
            }
        }
    }
}
