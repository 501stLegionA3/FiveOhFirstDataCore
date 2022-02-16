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

        public AlertModel(IAlertService aS)
        {
            alertService = aS;
        }

        public void SetTimer()
        {
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

    public interface IAlertBuilder
    {
        /// <summary>
        /// Set Timer Duration.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        IAlertBuilder SetDuration(int duration);
        /// <summary>
        /// Set message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IAlertBuilder SetMessage(string message);
        /// <summary>
        /// Set Alert Type
        /// </summary>
        /// <param name="alertType"></param>
        /// <returns></returns>
        IAlertBuilder SetType(AlertType alertType);
        /// <summary>
        /// Enable the Timer
        /// </summary>
        /// <param name="enableTimer"></param>
        /// <returns></returns>
        IAlertBuilder EnableTimer(bool enableTimer);
        /// <summary>
        /// Build Alert
        /// </summary>
        /// <returns></returns>
        AlertModel Build();
    }

    public class AlertBuilder : IAlertBuilder
    {
        private AlertModel Alert;

        public AlertBuilder(AlertService alertService)
        {
            this.Alert = new(alertService);
        }

        public AlertModel Build()
        {
            Alert.SetTimer();
            return Alert;
        }

        public IAlertBuilder EnableTimer(bool enableTimer)
        {
            Alert.IsRunning = enableTimer;
            return this;
        }

        public IAlertBuilder SetDuration(int duration)
        {
            Alert.Duration = duration;
            return this;
        }

        public IAlertBuilder SetMessage(string message)
        {
            Alert.Message = message;
            return this;
        }

        public IAlertBuilder SetType(AlertType alertType)
        {
            Alert.AlertType = alertType;
            return this;
        }
    }
}
