using System.ComponentModel.DataAnnotations;

namespace OzymandiasInvestments.Models
{
    public class Investments
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string UserId { get; set; }
        public decimal Shares { get; set; }
        public decimal OpenPrice { get; set; }
        public DateTime OpenTime { get; set; }
        public decimal? ClosePrice { get; set; }
        public DateTime? CloseTime { get; set; }
        public decimal? Profit { get; set; }
    }
}
