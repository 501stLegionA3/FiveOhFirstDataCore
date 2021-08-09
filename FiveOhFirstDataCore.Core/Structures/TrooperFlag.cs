using FiveOhFirstDataCore.Core.Account;

using System;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class TrooperFlag
    {
        public Guid FlagId { get; set; }
        public Trooper Author { get; set; }
        public int? AuthorId { get; set; }
        public Trooper FlagFor { get; set; }
        public int FlagForId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Contents { get; set; }
    }
}
