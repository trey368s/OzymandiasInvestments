namespace OzymandiasInvestments.Models.AppSettingsModels
{
    public class ConfigOptions
    {
        public AlpacaApiOptions AlpacaApiSettings { get; set; }
        public string alphaVantageKey { get; set; }
        public string alphaVantageKey2 { get; set; }
        public string gptKey { get; set; }
        public EmailSettingsModel EmailSettings { get; set; }
    }
}
