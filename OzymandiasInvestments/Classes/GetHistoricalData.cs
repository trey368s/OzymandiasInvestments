using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OzymandiasInvestments.Classes
{
    public class GetHistoricalData
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public GetHistoricalData(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        public async Task<IEnumerable<IBar>> GetHistoricalDataAsync(string symbol, DateTime start, DateTime end, BarTimeFrame timeframe)
        {
            var client = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(_apiKey, _apiSecret));

            var request = new HistoricalBarsRequest(symbol, start, end, timeframe);

            var page = await client.ListHistoricalBarsAsync(request);
            return page.Items;
        }
    }
}