using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;

using System.Collections.Generic;

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
