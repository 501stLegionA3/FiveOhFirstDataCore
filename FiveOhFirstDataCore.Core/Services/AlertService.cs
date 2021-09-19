using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Data.Services
{
    public class AlertService : IAlertService
    {
        public event EventHandler<AlertData> OnAlertPosted;
        public event EventHandler<RenderFragment> OnModalPosted;

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
                Level = Structures.Notice.AlertLevel.Danger
            });
        }

        public void PostAlert(object sender, string success)
            => PostAlert(sender, new AlertData()
            {
                Content = success,
                Level = Structures.Notice.AlertLevel.Success
            });

        public void PostModal(object sender, RenderFragment modalContent)
        {
            try
            {
                OnModalPosted.Invoke(sender, modalContent);
            }
            catch
            {
                // do nothing, as we have nothing listening.
            }
        }
    }
}
