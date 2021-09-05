using FiveOhFirstDataCore.Core.Account;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Message
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
