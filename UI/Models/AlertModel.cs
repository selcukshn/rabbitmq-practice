using UI.Enums;

namespace UI.Models
{
    public class AlertModel
    {
        public EAlertType Type { get; set; }
        public string Message { get; set; }
        public AlertModel(EAlertType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}