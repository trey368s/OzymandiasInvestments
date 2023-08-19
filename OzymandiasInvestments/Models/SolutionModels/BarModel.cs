using System;
using Alpaca.Markets;

namespace OzymandiasInvestments.Models.SolutionModels
{
    public class BarModel : IBar
    {
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public decimal Vwap { get; set; }
        public ulong TradeCount { get; set; }
        public DateTime TimeUtc { get; set; }
        public string Symbol { get; set; }
        public long UnixTime { get; set; }



    }
}