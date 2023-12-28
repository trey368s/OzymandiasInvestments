﻿using Alpaca.Markets;
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

namespace OzymandiasInvestments.Classes
{
    public class GetMarketData
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _alphaVantageKey;
        private readonly object locker = new object();
        private readonly Dictionary<string, List<ITrade>> minuteBarsData = new Dictionary<string, List<ITrade>>();

        public GetMarketData(string apiKey, string apiSecret, string alphaVantageKey)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _alphaVantageKey = alphaVantageKey;
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
            string alphaVantageApiKey = _alphaVantageKey;
            string symbol = ticker.ToUpper();
            string endpoint = $"https://www.alphavantage.co/query";
            string function = "OVERVIEW";

            string queryString = $"?function={function}&symbol={symbol}&apikey={alphaVantageApiKey}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(endpoint + queryString);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    dynamic companyData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseData);
                    string companyName = companyData.Name ?? "N/A";
                    string marketCap = companyData.MarketCapitalization ?? "N/A";
                    string description = companyData.Description ?? "N/A";
                    string sector = companyData.Sector ?? "N/A";
                    string industry = companyData.Industry ?? "N/A";
                    string trailingPE = companyData.TrailingPE ?? "N/A";
                    string eps = companyData.EPS ?? "N/A";
                    string targetPrice = companyData.AnalystTargetPrice?.Round(2)?.ToString("0.00") ?? "N/A";
                    string yearHigh = companyData["52WeekHigh"]?.Round(2)?.ToString("0.00") ?? "N/A";
                    string yearLow = companyData["52WeekLow"]?.Round(2)?.ToString("0.00") ?? "N/A";
                    string movingAverage = companyData["50DayMovingAverage"]?.Round(2)?.ToString("0.00") ?? "N/A";

                    DetailedInfoModel info = new DetailedInfoModel
                    {
                        marketCap = ShortenNumber(marketCap),
                        companyName = companyName,
                        description = description,
                        sector = sector,
                        industry = industry,
                        yearHigh = yearHigh,
                        yearLow = yearLow,
                        targetPrice = targetPrice,
                        movingAverage = movingAverage,
                        eps = eps,
                        trailingPE = trailingPE
                    };
                    return info;
                }
                else
                {
                    return new DetailedInfoModel { };
                    //todo
                }
            }
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
    }
}