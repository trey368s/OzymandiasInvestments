using Alpaca.Markets;
using OzymandiasInvestments.Migrations.InvestmentDb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;
using Alpaca.Markets.Extensions;
using System.Linq;
using OzymandiasInvestments.Models.SolutionModels;
using MessagePack.Formatters;

namespace OzymandiasInvestments.Classes
{
    public class GetMarketData
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _alphaVantageKey;
        private readonly string _alphaVantageKey2;
        private readonly object locker = new object();
        private readonly Dictionary<string, List<ITrade>> minuteBarsData = new Dictionary<string, List<ITrade>>();

        public GetMarketData(string apiKey, string apiSecret, string alphaVantageKey, string alphaVantageKey2)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _alphaVantageKey = alphaVantageKey;
            _alphaVantageKey2 = alphaVantageKey2;
        }

        public async Task<IEnumerable<IBar>> GetHistoricalDataAsync(string symbol, DateTime start, DateTime end, BarTimeFrame timeframe)
        {
            var client = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(_apiKey, _apiSecret));
            var request = new HistoricalBarsRequest(symbol, start, end, timeframe);
            var page = await client.ListHistoricalBarsAsync(request);
            return page.Items;
        }

        public async Task<IEnumerable<IBar>> GetSmaDataAsync(string symbol, DateTime start, DateTime end, BarTimeFrame timeframe)
        {
            var client = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(_apiKey, _apiSecret));
            var request = new HistoricalBarsRequest(symbol, start, end, timeframe);
            var bars = new List<IBar>();
            await foreach (var bar in client.GetSimpleMovingAverageAsync<HistoricalBarsRequest>(request, 50))
            {
                bars.Add(bar);
            }
            return bars;
        }

        internal async Task<List<INewsArticle>> GetNewsAsync(NewsArticlesRequest newsRequest)
        {
            var client = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(_apiKey, _apiSecret));
            var newsArticlesAsyncEnumerable = client.GetNewsArticlesAsAsyncEnumerable(newsRequest);
            var newsArticlesList = new List<INewsArticle>();
            var counter = 0;

            await foreach (var newsArticle in newsArticlesAsyncEnumerable)
            {
                newsArticlesList.Add(newsArticle);
                counter++;
                if (counter >= 6)
                {
                    break;
                }
            }
            return newsArticlesList;
        }
        internal async Task<DetailedInfoModel> GetDetailedCompanyInfo(string ticker)
        {
            string[] alphaVantageApiKeys = { _alphaVantageKey, _alphaVantageKey2 };
            string symbol = ticker.ToUpper();
            string endpoint = "https://www.alphavantage.co/query";
            string function = "OVERVIEW";

            foreach (string apiKey in alphaVantageApiKeys)
            {
                DetailedInfoModel info = await GetCompanyInfoWithApiKey(endpoint, function, symbol, apiKey);

                if (info.companyName != "N/A")
                {
                    return info;
                }
            }

            return new DetailedInfoModel { };
        }

        private async Task<DetailedInfoModel> GetCompanyInfoWithApiKey(string endpoint, string function, string symbol, string apiKey)
        {
            string queryString = $"?function={function}&symbol={symbol}&apikey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(endpoint + queryString);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    dynamic companyData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseData);
                    string marketCap = companyData.MarketCapitalization;

                    return new DetailedInfoModel
                    {
                        marketCap = ShortenNumber(marketCap) ?? "N/A",
                        companyName = companyData.Name ?? "N/A",
                        description = companyData.Description ?? "N/A",
                        sector = companyData.Sector ?? "N/A",
                        industry = companyData.Industry ?? "N/A",
                        yearHigh = FormatNumericValue(companyData["52WeekHigh"]) ?? "N/A",
                        yearLow = FormatNumericValue(companyData["52WeekLow"]) ?? "N/A",
                        targetPrice = FormatNumericValue(companyData["AnalystTargetPrice"]) ?? "N/A",
                        movingAverage = FormatNumericValue(companyData["50DayMovingAverage"]) ?? "N/A",
                        eps = companyData.EPS ?? "N/A",
                        trailingPE = companyData.TrailingPE ?? "N/A"
                    };
                }
            }

            return new DetailedInfoModel { };
        }

        internal string ShortenNumber(string input)
        {
            if (double.TryParse(input, out double number))
            {
                if (number >= 1_000_000_000_000)
                    return $"{Math.Round(number / 1_000_000_000_000, 2)} T";
                if (number >= 1_000_000_000)
                    return $"{Math.Round(number / 1_000_000_000, 2)} B";
                if (number >= 1_000_000)
                    return $"{Math.Round(number / 1_000_000, 2)} M";
                if (number >= 1_000)
                    return $"{Math.Round(number / 1_000, 2)} K";

                return Math.Round(number, 2).ToString();
            }

            return "N/A";
        }

        string FormatNumericValue(object value)
        {
            if (value != null && decimal.TryParse(value.ToString(), out decimal numericValue))
            {
                return numericValue.ToString("0.00");
            }
            else
            {
                return "N/A";
            }
        }
    }
}