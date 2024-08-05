using Alpaca.Markets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using OzymandiasInvestments.Migrations.InvestmentDb;
using OzymandiasInvestments.Models.SolutionModels;
using System.Data;

namespace OzymandiasInvestments.Classes
{
    public class GetPositionData
    {
        public async Task<DataTable> GetPositions(string apiKey, string apiSecret)
        {
            var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(apiKey, apiSecret));
            var positions = await client.ListPositionsAsync();
            return ConvertToDataTable(positions);
        }

        public async Task<List<EquityModel>> GetEquity(string apiKey, string apiSecret)
        {
            var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(apiKey, apiSecret));
            var requestAccount = new PortfolioHistoryRequest
            {
                Period = new HistoryPeriod(365, HistoryPeriodUnit.Day)
            };
            var portfolio = await client.GetPortfolioHistoryAsync(requestAccount);
            var accountList = portfolio.Items.ToList();
            var requestCalander = new CalendarRequest(DateOnly.FromDateTime(DateTime.Now.AddYears(-1)), DateOnly.FromDateTime(DateTime.Now));
            var calander = await client.ListCalendarAsync(requestCalander);
            
            var equityListModel = new List<EquityModel>();
            var counter = 0;
            foreach (var equity in accountList)
            {
                try
                {
                    var equitySnapshot = new EquityModel
                    {
                        TradingDay = DateOnly.FromDateTime(calander[counter].TradingDateUtc).ToLongDateString(),
                        Equity = equity.Equity.ToString()
                    };
                    counter++;
                    equityListModel.Add(equitySnapshot);
                }
                catch
                {
                    break;
                }
            }
            return equityListModel;
        }

        public async Task<AccountInfoModel> GetAccountInfo(string apiKey, string apiSecret)
        {
            var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(apiKey, apiSecret));
            var accountInfo = await client.GetAccountAsync();
            var accountValue = accountInfo.Equity.ToString();
            string formmatedAccountValue = string.Format("{0:N}", decimal.Parse(accountValue));
            var info = new AccountInfoModel
            {
                cash = accountInfo.TradableCash.ToString(),
                longValue = accountInfo.LongMarketValue.ToString(),
                shortValue = accountInfo.ShortMarketValue.ToString(),
                accountValue = formmatedAccountValue,
                yesterdayAccountValue = accountInfo.LastEquity.ToString()
            };
            return info;
        }


        public DataTable ConvertToDataTable(IEnumerable<IPosition> positions)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Symbol", typeof(string));
            dataTable.Columns.Add("Current Price", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(string));
            dataTable.Columns.Add("Market Value", typeof(string));
            dataTable.Columns.Add("Cost Basis", typeof(string));
            dataTable.Columns.Add("Total P/L (%)", typeof(string));
            dataTable.Columns.Add("Total P/L ($)", typeof(string));

            foreach (var position in positions)
            {
                DataRow row = dataTable.NewRow();
                row["Symbol"] = position.Symbol;
                row["Current Price"] = "$" + Math.Round((decimal)position.AssetLastPrice,2).ToString("0.00");
                row["Quantity"] = position.Quantity;
                row["Market Value"] = "$" + Math.Round((decimal)position.MarketValue,2).ToString("0.00");
                row["Cost Basis"] = "$" + Math.Round(position.CostBasis,2).ToString("0.00");
                row["Total P/L (%)"] = Math.Round((decimal)position.UnrealizedProfitLossPercent, 2).ToString("0.00") + "%";
                row["Total P/L ($)"] = "$" + Math.Round((decimal)position.UnrealizedProfitLoss, 2).ToString("0.00");
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
