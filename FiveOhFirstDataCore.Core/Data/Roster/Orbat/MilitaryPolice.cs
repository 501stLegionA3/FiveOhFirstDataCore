using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class MilitaryPolice : IAssignable<Trooper>
    {
        public List<Trooper> CoLeadMP { get; set; } = new();
        public List<Trooper> MP { get; set; } = new();

        public void Assign(Trooper item)
        {
            throw new System.NotImplementedException();
        }
    }
}
