using ProjectDataCore.Data.Services.Alert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Alert
{
    public class AlertModel : IDisposable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public AlertType AlertType { get; set; }
        public string Message { get; set; } = "";
        /// <summary>
        /// Duration in ms
        /// </summary>
        public int Duration { get; set; } = 3500;
        public System.Timers.Timer? Timer { get; set; }
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        public bool IsRunning { get; set; } = false;

        private readonly IAlertService alertService;

        public AlertModel(AlertType alertType, string message, IAlertService aS, bool enableTimer = false, int duration = 3500)
        {
            AlertType = alertType;
            Message = message;
            Duration = duration;
            IsRunning = enableTimer;
            alertService = aS;
            if (IsRunning)
            {
                Timer = new System.Timers.Timer(Duration);
                Timer.Elapsed += TimerElapsed;
            }
        }

        private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            alertService.DeleteAlert(this);
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
