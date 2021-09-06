using FiveOhFirstDataCore.Core.Structures;

namespace FiveOhFirstDataCore.Core.Services
{
    public class AlertService : IAlertService
    {
        public event EventHandler<AlertData> OnAlertPosted;

        public void PostAlert(object sender, AlertData alert)
        {
            try
            {
                OnAlertPosted.Invoke(sender, alert);
            }
            catch
            {
                // do nothing, as we have nothing listening.
            }
        }

        public void PostAlert(object sender, List<string> errors)
        {
            string data = "<strong>Something went wrong!</strong>\n" +
                "<ul>";
            foreach (var i in errors)
            {
                data += $"<li>{i}</li>\n";
            }
            data += "</ul>";

            PostAlert(sender, new AlertData()
            {
                Content = data,
                Level = Data.Notice.AlertLevel.Danger
            });
        }

        public void PostAlert(object sender, string success)
            => PostAlert(sender, new AlertData()
            {
                Content = success,
                Level = Data.Notice.AlertLevel.Success
            });
    }
}
