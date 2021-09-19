using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures
{
    public class TrooperDescription
    {
        public Guid Id { get; set; }
        public Trooper Author { get; set; }
        public int? AuthorId { get; set; }
        public Trooper DescriptionFor { get; set; }
        public int DescriptionForId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
    }
}
