using Alpaca.Markets;
using System.Data;

namespace OzymandiasInvestments.Classes
{
    public class GetOrderData
    {
        public async Task<DataTable> GetOrders(string apiKey, string apiSecret)
        {
            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaTradingClient(new SecretKey(apiKey, apiSecret));
            var orders = await client.ListOrdersAsync(
                new ListOrdersRequest
                {
                    LimitOrderNumber = 100,
                    OrderStatusFilter = OrderStatusFilter.All
                });
            return ConvertToDataTable(orders);
        }

        public async Task<IOrder> CreateOrderAsync(string symbol, OrderQuantity quantity, OrderSide side, OrderType orderType, decimal? limitPrice, decimal? stopPrice, TrailOffset trailOffset, TimeInForce timeInForce, string ApiKey, string SecretKey)
        {
            var client = Alpaca.Markets.Environments.Paper.GetAlpacaTradingClient(new SecretKey(ApiKey, SecretKey));

            IOrder order = null;
            switch (orderType)
            {
                case OrderType.Market:
                    order = await client.PostOrderAsync(new NewOrderRequest(symbol, quantity, side, orderType, timeInForce));
                    break;
                case OrderType.Limit:
                    order = await client.PostOrderAsync(new NewOrderRequest(symbol, quantity, side, orderType, timeInForce) { LimitPrice = limitPrice });
                    break;
                case OrderType.Stop:
                    order = await client.PostOrderAsync(new NewOrderRequest(symbol, quantity, side, orderType, timeInForce) { StopPrice = stopPrice });
                    break;
                case OrderType.StopLimit:
                    order = await client.PostOrderAsync(new NewOrderRequest(symbol, quantity, side, orderType, timeInForce) { StopPrice = stopPrice, LimitPrice = limitPrice });
                    break;
                case OrderType.TrailingStop:
                    if (side == OrderSide.Buy)
                    {
                        order = await client.PostOrderAsync(TrailingStopOrder.Buy(symbol, quantity, trailOffset));
                    }
                    if (side == OrderSide.Sell)
                    {
                        order = await client.PostOrderAsync(TrailingStopOrder.Sell(symbol, quantity, trailOffset));
                    }
                    break;
                default:
                    {
                        throw new ArgumentException("Invalid order type. Supported types: 'market', 'limit', 'stop', 'stop_limit', 'trailing_stop'");
                    }
            }
            return order;
        }

        public DataTable ConvertToDataTable(IEnumerable<IOrder> orders)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Symbol", typeof(string));
            dataTable.Columns.Add("Side", typeof(string));
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(string));
            dataTable.Columns.Add("Filled Quantity", typeof(string));
            dataTable.Columns.Add("Filled Price", typeof(string));
            dataTable.Columns.Add("Limit Price", typeof(string));
            dataTable.Columns.Add("Stop Price", typeof(string));
            dataTable.Columns.Add("Submitted At UTC", typeof(DateTime));
            dataTable.Columns.Add("Filled At UTC", typeof(DateTime));

            foreach (var order in orders)
            {
                DataRow row = dataTable.NewRow();
                row["Symbol"] = order.Symbol;
                row["Side"] = order.OrderSide;
                row["Type"] = AddSpaces(order.OrderType);
                row["Status"] = order.OrderStatus;
                row["Quantity"] = order.Quantity;
                row["Filled Quantity"] = order.FilledQuantity;
                row["Filled Price"] = order.AverageFillPrice.HasValue ? "$" + Math.Round((decimal)order.AverageFillPrice,2).ToString("0.00") : DBNull.Value;
                row["Limit Price"] = order.LimitPrice.HasValue ? "$" + Math.Round((decimal)order.LimitPrice,2).ToString("0.00") : DBNull.Value;
                row["Stop Price"] = order.StopPrice.HasValue ? "$" + Math.Round((decimal)order.StopPrice,2).ToString("0.00") : DBNull.Value;
                row["Submitted At UTC"] = order.SubmittedAtUtc.HasValue ? (object)order.SubmittedAtUtc : DBNull.Value;
                row["Filled At UTC"] = order.FilledAtUtc.HasValue ? (object)order.FilledAtUtc : DBNull.Value;
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static string AddSpaces(OrderType input)
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