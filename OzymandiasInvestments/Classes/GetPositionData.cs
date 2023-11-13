using Alpaca.Markets;
using System.Data;

namespace OzymandiasInvestments.Classes
{
    public class GetPositionData
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        public GetPositionData(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        public async Task<DataTable> GetPositions()
        {
            var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(_apiKey, _apiSecret));
            var positions = await client.ListPositionsAsync();
            return ConvertToDataTable(positions);
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
