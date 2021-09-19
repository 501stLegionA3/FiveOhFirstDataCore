namespace FiveOhFirstDataCore.Data.Mail
{
    public class MailConfiguration
    {
        public string Client { get; set; }
        public int Port { get; set; }
        public bool RequireLogin { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
