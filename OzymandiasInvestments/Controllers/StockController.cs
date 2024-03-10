using Microsoft.AspNetCore.Mvc;
using OzymandiasInvestments.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using OzymandiasInvestments.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Alpaca.Markets;
using OzymandiasInvestments.Classes;
using OzymandiasInvestments.Models.SolutionModels;
using OzymandiasInvestments.Migrations.InvestmentDb;

namespace OzymandiasInvestments.Controllers
{
    public class StockController : Controller
    {
        private readonly ILogger<StockController> _logger;
        private readonly InvestmentDbContext _dbContext;
        private readonly UserManager<OzymandiasInvestmentsUser> _userManager;
        private readonly GetMarketData _historicalData;
        private readonly GetOrderData _orderData;
        private readonly GetPositionData _positionData;
        private readonly GetActivityData _activityData;
        private readonly SendEmail _sendEmail;
        private readonly GetChatGPTResponse _gptResponse;
        private IEnumerable<IBar> _bars;

        public StockController(UserManager<OzymandiasInvestmentsUser> userManager, ILogger<StockController> logger,
            InvestmentDbContext dbContext, GetMarketData historicalData, GetPositionData positionData, 
            GetOrderData orderData, GetActivityData activityData, SendEmail sendEmail, GetChatGPTResponse gptResponse)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
            _historicalData = historicalData;
            _positionData = positionData;
            _orderData = orderData;
            _activityData = activityData;
            _sendEmail = sendEmail;
            _gptResponse = gptResponse;
        }

        public IActionResult Startpage()
        {
            return Redirect("https://stegeman.dev");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new HistoricalDataModel
            {
                Bars = Enumerable.Empty<IBar>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string ticker)
        {
            string symbol = ticker.ToUpper().Trim() ?? "SPY";
            DateTime start = DateTime.Now.AddMonths(-12);
            DateTime end = DateTime.Today.AddMinutes(-15);
            var timeframe = BarTimeFrame.Day;
            
            var bars = await _historicalData.GetHistoricalDataAsync(symbol, start, end, timeframe);
            var recentBars = bars.OrderByDescending(b => b.TimeUtc).Take(10);

            var volume = recentBars.Any() ? recentBars.Average(b => b.Volume) : 0;
            var averageVolume = _historicalData.ShortenNumber(volume.ToString());

            var symbolsList = new List<string>();
            symbolsList.Add(ticker.ToUpper().Trim());
            var request = new NewsArticlesRequest(symbolsList);
            var news = await _historicalData.GetNewsAsync(request);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var investments = _dbContext.Investment
                .Where(w => w.UserId == userId && w.Symbol == ticker.ToUpper().Trim())
                .OrderByDescending(o => o.OpenTime)
                .ToList();

            var info = await _historicalData.GetDetailedCompanyInfo(symbol);

            var sma = await _historicalData.GetSmaDataAsync(symbol, start, end, timeframe);

            var newsSentiment = await _gptResponse.SentimentChatBot(news, ticker);
            
            var viewModel = new HistoricalDataModel
            {
                Bars = bars,
                articles = newsSentiment,
                Investments = investments,
                detailedInfo = info,
                averageVolume = averageVolume,
                sma = sma
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Investments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var investments = _dbContext.Investment
                .Where(w => w.UserId == userId)
                .OrderByDescending(o => o.OpenTime)
                .ToList();
            return View(investments);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddInvestment()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddInvestment(decimal openPrice, string symbol, decimal shares, DateTime dateTime)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var investment = new InvestmentsModel()
            {
                UserId = userId,
                Symbol = symbol,
                Shares = shares,
                OpenPrice = openPrice,
                OpenTime = dateTime,
                ClosePrice = null,
                CloseTime = null,
                Profit = null
            };

            _dbContext.Investment.Add(investment);
            _dbContext.SaveChanges();

            return RedirectToAction("Portfolio");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CloseInvestment(int id)
        {
            var investment = await _dbContext.Investment.FindAsync(id);

            if (investment != null)
            {
                InvestmentsModel model = new InvestmentsModel
                {
                    Id = investment.Id,
                    Symbol = investment.Symbol,
                    Shares = investment.Shares,
                    OpenPrice = investment.OpenPrice,
                    OpenTime = investment.OpenTime,
                    UserId = investment.UserId
                };

                return View(model);
            }

            return RedirectToAction("Portfolio");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CloseInvestment(int id, decimal closePrice, DateTime closeTime)
        {
            var investment = await _dbContext.Investment.FindAsync(id);

            if (investment != null)
            {
                investment.ClosePrice = closePrice;
                investment.CloseTime = closeTime;
                investment.Profit = (investment.ClosePrice * investment.Shares) - (investment.OpenPrice * investment.Shares);

                _dbContext.Update(investment);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Portfolio");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateInvestment(int id)
        {
            var investment = await _dbContext.Investment.FindAsync(id);

            if (investment != null)
            {
                InvestmentsModel model = new InvestmentsModel
                {
                    Id = investment.Id,
                    Symbol = investment.Symbol,
                    Shares = investment.Shares,
                    OpenPrice = investment.OpenPrice,
                    OpenTime = investment.OpenTime,
                    ClosePrice = investment.ClosePrice,
                    CloseTime = investment.CloseTime,
                    UserId = investment.UserId
                };

                return View(model);
            }

            return RedirectToAction("Portfolio");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateInvestment(InvestmentsModel model)
        {
            if (ModelState.IsValid)
            {
                var existingInvestment = await _dbContext.Investment.FindAsync(model.Id);
                if (existingInvestment != null)
                {
                    existingInvestment.Id = model.Id;
                    existingInvestment.Symbol = model.Symbol;
                    existingInvestment.Shares = model.Shares;
                    existingInvestment.OpenPrice = model.OpenPrice;
                    existingInvestment.OpenTime = model.OpenTime;
                    existingInvestment.ClosePrice = model.ClosePrice;
                    existingInvestment.CloseTime = model.CloseTime;
                    existingInvestment.UserId = model.UserId;
                    existingInvestment.Profit = (existingInvestment.ClosePrice * existingInvestment.Shares) - (existingInvestment.OpenPrice * existingInvestment.Shares);

                    _dbContext.Update(existingInvestment);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Portfolio");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Investment not found.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteInvestment(int id)
        {
            var investment = await _dbContext.Investment.FindAsync(id);
            if (investment != null)
            {
                _dbContext.Remove(investment);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Portfolio");
            }
            return NotFound();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Positions()
        {
            var requestPositions = await _positionData.GetPositions();
            var requestAccountInfo = await _positionData.GetAccountInfo();
            var requestEquityInfo = await _positionData.GetEquity();
            var request = new PositionModel
            {
                info = requestAccountInfo,
                positions = requestPositions,
                equityModelList = requestEquityInfo
            };
            return View(request);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var request = await _orderData.GetOrders();
            return View(request);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Activities()
        {
            var request = await _activityData.GetActivities();
            return View(request);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(string symbol, decimal quantity, OrderSide side, OrderType orderType, decimal? limitPrice, decimal? stopPrice, decimal trailOffset, TimeInForce timeInForce, string quantityType, string trailType)
        {
            OrderQuantity qty = OrderQuantity.Fractional(quantity);
            if (quantityType == "Dollars")
            {
                qty = OrderQuantity.Notional(quantity);
            }
            TrailOffset offset = TrailOffset.InDollars(trailOffset);
            if(trailType == "Percent %")
            {
                offset = TrailOffset.InPercent(trailOffset);
            }
            var request = await _orderData.CreateOrderAsync(symbol, qty, side, orderType, limitPrice, stopPrice, offset, timeInForce);
            return RedirectToAction("Orders");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}