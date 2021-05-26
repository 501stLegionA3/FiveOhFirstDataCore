using FiveOhFirstDataCore.Core.Account;
using Markdig;
using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Notice
{
    public class Notice
    {
        public Guid NoticeId { get; set; }
        public Trooper Author { get; set; }
        public int AuthorId { get; set; }
        public NoticeBoardData NoticeBoard { get; set; }
        public string NoticeBoardName { get; set; }
        public DateTime PostedOn { get; set; }
        public bool Sticky { get; set; }
        public string Contents { get; set; } = "";
        public MarkupString Display => new(Markdown.ToHtml(Contents));
        public NoticeLevel Level { get; set; } = NoticeLevel.Primary;
    }
}
