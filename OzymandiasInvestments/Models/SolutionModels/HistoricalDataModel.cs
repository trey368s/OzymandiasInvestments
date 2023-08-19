using Alpaca.Markets;

namespace OzymandiasInvestments.Models.SolutionModels
{
    public class HistoricalDataModel
    {
        public IEnumerable<IBar>? Bars { get; set; }
        public List<IBar> listBars { get; set; }
        public List<INewsArticle> articles { get; set; }
        public List<InvestmentsModel> Investments { get; set; }
    }
}
