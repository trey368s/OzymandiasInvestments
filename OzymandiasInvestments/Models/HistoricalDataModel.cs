using Alpaca.Markets;

namespace OzymandiasInvestments.Models
{
    public class HistoricalDataModel
    {
        public IEnumerable<IBar>? Bars { get; set; }
        public List<IBar> listBars { get; set; }
        public List<INewsArticle> articles { get; set; }
        public List<Investments> Investments { get; set; }
    }
}
