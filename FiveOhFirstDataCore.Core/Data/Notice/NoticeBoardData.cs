using System.Collections.Generic;

namespace FiveOhFirstDataCore.Core.Data.Notice
{
    public class NoticeBoardData
    {
        public string Location { get; set; }
        public List<Notice> Notices { get; set; } = new();
    }
}
