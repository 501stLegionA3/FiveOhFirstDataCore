using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Notice
{
    public class NoticeBoardData
    {
        public string Location { get; set; }
        public List<Notice> Notices { get; set; } = new();
    }
}
