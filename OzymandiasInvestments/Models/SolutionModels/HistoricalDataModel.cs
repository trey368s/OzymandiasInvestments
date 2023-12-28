using Alpaca.Markets;

namespace OzymandiasInvestments.Models.SolutionModels
{
    public class HistoricalDataModel
    {
        public IEnumerable<IBar>? Bars { get; set; }
        public IEnumerable<IBar>? sma { get; set; }
        public List<INewsArticle> articles { get; set; }
        public List<InvestmentsModel> Investments { get; set; }
        public DetailedInfoModel detailedInfo { get; set; }
        public string averageVolume { get; set; }
    }
}
