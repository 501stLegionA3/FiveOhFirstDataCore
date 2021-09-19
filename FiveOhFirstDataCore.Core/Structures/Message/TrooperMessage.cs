using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Data.Structures.Message
{
    public class TrooperMessage
    {
        public Guid Key { get; set; }
        public Trooper Author { get; set; }
        public int AuthorId { get; set; }

        public string Message { get; set; }
        public MarkupString Display => new(Message ?? "");
        public DateTime CreatedOn { get; set; }
        public Guid MessageFor { get; set; }
    }
}
