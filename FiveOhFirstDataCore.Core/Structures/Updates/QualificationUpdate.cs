using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;

namespace FiveOhFirstDataCore.Core.Structures.Updates
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
