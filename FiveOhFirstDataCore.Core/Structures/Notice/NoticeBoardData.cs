namespace FiveOhFirstDataCore.Data.Structures.Notice
{
    public class NoticeBoardData
    {
        public string Location { get; set; }
        public List<Notice> Notices { get; set; } = new();
    }
}
