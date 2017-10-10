namespace Rouse.PatentCalculator.Web.Models
{
    public class Alert {
        public const string TEMP_ALERT_KEY = "TempAlertKey";
        public string AlertType { get; set; }
        public string Message { get; set; }
        public string Tag { get; set; }
    }

    public class AlertType {
        public const string SUCCESS = "success";
        public const string DANGER = "danger";
    }
    public class AlertTag {
        public const string DATA_EMPTY = "EmptyData";
    }
}