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

namespace OzymandiasInvestments.Controllers
{
    public class StockController : Controller
    {
        private readonly ILogger<StockController> _logger;
        private readonly InvestmentDbContext _dbContext;
        private readonly UserManager<OzymandiasInvestmentsUser> _userManager;
        private readonly GetMarketData _historicalData;
        private IEnumerable<IBar> _bars;

        public StockController(UserManager<OzymandiasInvestmentsUser> userManager,
            ILogger<StockController> logger,
            InvestmentDbContext dbContext, GetMarketData historicalData)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
            _historicalData = historicalData;
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
            string symbol = ticker ?? "AAPL";
            DateTime start = DateTime.Now.AddMonths(-3);
            DateTime end = DateTime.Today;
            var timeframe = BarTimeFrame.Day;

            var bars = await _historicalData.GetHistoricalDataAsync(symbol, start, end, timeframe);
            var viewModel = new HistoricalDataModel
            {
                Bars = bars
            };

            //await _historicalData.AddRealTimeBarDataAsync(viewModel, symbol);

            return View("Index", viewModel);
        }

        public IActionResult Startpage()
        {
            return Redirect("https://stegeman.dev");
        }

        [Authorize]
        public IActionResult AddInvestment()
        {
            return View();
        }

        [Authorize]
        public IActionResult Portfolio()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var investments = _dbContext.Investment
                .Where(w => w.UserId == userId)
                .OrderByDescending(o => o.OpenTime)
                .ToList();
            return View(investments);
        }

        [HttpPost]
        public async Task<IActionResult> AddInvestment(decimal openPrice, string symbol, decimal shares, DateTime dateTime)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var investment = new Investments()
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

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CloseInvestment(int id)
        {
            var investment = await _dbContext.Investment.FindAsync(id);

            if (investment != null)
            {
                Investments model = new Investments
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
        public async Task<IActionResult> UpdateInvestment(int id)
        {
            var investment = await _dbContext.Investment.FindAsync(id);

            if (investment != null)
            {
                Investments model = new Investments
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
        public async Task<IActionResult> UpdateInvestment(Investments model)
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
                    existingInvestment.Profit = (existingInvestment.ClosePrice * existingInvestment.Shares)-(existingInvestment.OpenPrice * existingInvestment.Shares);

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}