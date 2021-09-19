using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public class QualificationUpdate : UpdateBase
    {
        public Qualification Added { get; set; }
        public Qualification Removed { get; set; }
        public bool Revoked { get; set; } = false;

        public Qualification OldQualifications { get; set; }

        public List<Trooper> Instructors { get; set; }
    }
}
