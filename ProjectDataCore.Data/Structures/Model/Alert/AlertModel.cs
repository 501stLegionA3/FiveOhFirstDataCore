using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Alert
{
    public class AlertModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public AlertType AlertType { get; set; }
        public string Message { get; set; } = "";

        public AlertModel(AlertType alertType, string message)
        {
            AlertType = alertType;
            Message = message;
        }
    }
}
