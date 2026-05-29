namespace documentvaultapi.RbbitMQ.Models
{
    public class HangfireJobSettings
    {
        public List<DynamicJobSchedule> Jobs { get; set; } = new();
        public string TimeZone { get; set; } = "UTC";
    }

    public class DynamicJobSchedule
    {
        public string JobName { get; set; }
        public string Method { get; set; }
        public string Cron { get; set; }
        public bool Enabled { get; set; }
    }
}
