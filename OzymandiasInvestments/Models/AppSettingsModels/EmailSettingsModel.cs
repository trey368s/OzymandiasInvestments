namespace OzymandiasInvestments.Models.AppSettingsModels
{
    public class EmailSettingsModel
    {
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set;}
        public string SenderName { get; set; }
        public string SenderEmail { get; set;}
        public string smtpUsername { get; set; }
        public string smtpPassword { get; set;}
    }
}
