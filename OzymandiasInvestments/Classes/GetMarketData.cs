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
        private readonly object locker = new object();
        private readonly Dictionary<string, List<ITrade>> minuteBarsData = new Dictionary<string, List<ITrade>>();

        public GetMarketData(string apiKey, string apiSecret)
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

        internal async Task AddRealTimeBarDataAsync(HistoricalDataModel viewModel, string symbol)
        {
            using var client = Alpaca.Markets.Environments.Live.GetAlpacaDataStreamingClient(new SecretKey(_apiKey, _apiSecret));
            await client.ConnectAndAuthenticateAsync();
            var tradeSubscription = client.GetTradeSubscription("AAPL");
            tradeSubscription.Received += (trade) =>
            {
                lock (locker)
                {
                    if (!minuteBarsData.TryGetValue(symbol, out var minuteTrades))
                    {
                        minuteTrades = new List<ITrade>();
                        minuteBarsData[symbol] = minuteTrades;
                    }
                    minuteTrades.Add(trade);
                    var currentMinute = DateTime.UtcNow.Minute;
                    var barTime = trade.TimestampUtc;
                    if (minuteTrades.Count > 0 && barTime.Minute != currentMinute)
                    {
                        var newBar = new BarModel
                        {
                            Time = new DateTime(barTime.Year, barTime.Month, barTime.Day, barTime.Hour, currentMinute, 0),
                            Open = minuteTrades.First().Price,
                            High = minuteTrades.Max(t => t.Price),
                            Low = minuteTrades.Min(t => t.Price),
                            Close = minuteTrades.Last().Price,
                            Volume = minuteTrades.Sum(t => t.Size)
                        };
                        viewModel.listBars.Add(newBar);
                        minuteTrades.Clear();
                    }
                }
            };
            await client.SubscribeAsync(tradeSubscription);
        }
    }
}