namespace AutoParts.Web.Configuration
{
    public class ReportEmailSettings
    {
        public string Recipient { get; set; } = "";
        public string Sender { get; set; } = "";
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = "";
        public string SmtpPass { get; set; } = "";
    }
}
