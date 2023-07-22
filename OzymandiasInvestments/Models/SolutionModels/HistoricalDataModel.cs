using Alpaca.Markets;

namespace OzymandiasInvestments.Models.SolutionModels
{
    public class HistoricalDataModel
    {
        public IEnumerable<IBar>? Bars { get; set; }
    }
}
