using Alpaca.Markets;
using System.Data;

namespace OzymandiasInvestments.Classes
{
    public class GetActivityData
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        public GetActivityData(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        public async Task<DataTable> GetActivities()
        {
            var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(_apiKey, _apiSecret));
            var request = new AccountActivitiesRequest();
            var activities = await client.ListAccountActivitiesAsync(request);
            return ConvertToDataTable(activities);
        }

        public DataTable ConvertToDataTable(IReadOnlyList<IAccountActivity> activities)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Amount", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(string));
            dataTable.Columns.Add("Date Time UTC", typeof(string));

            foreach (var activity in activities)
            {
                DataRow row = dataTable.NewRow();
                row["Description"] = activity.Side + " " + activity.CumulativeQuantity.ToString() + " " + activity.Symbol;
                row["Type"] = AddSpaces(activity.Type);
                row["Amount"] = "$" + Math.Round((decimal)(activity.Price * activity.CumulativeQuantity), 2);
                row["Quantity"] = activity.CumulativeQuantity;
                row["Date Time UTC"] = activity.ActivityDateTimeUtc;
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static string AddSpaces(TradeEvent? input)
        {
            string output = "";
            string strInput = input.ToString();
            for (int i = 0; i < strInput.Length; i++)
            {
                char c = strInput[i];
                if (char.IsUpper(c) && i > 0)
                {
                    output += " ";
                }
                output += c;
            }
            return output;
        }
    }
}
