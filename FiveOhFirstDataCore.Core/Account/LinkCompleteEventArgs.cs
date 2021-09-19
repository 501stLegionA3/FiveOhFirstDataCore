namespace FiveOhFirstDataCore.Data.Account
{
    public class LinkCompleteEventArgs
    {
        public bool Complete { get; internal set; }
        public ulong Guild { get; internal set; }
        public Trooper? LinkedTrooper { get; internal set; }
    }
}
