using Alpaca.Markets;

namespace OzymandiasInvestments.Models.SolutionModels
{
    public class NewsArticleModel
    {
        public INewsArticle newsArticle { get; set; }
        public SentimentModel sentiment { get; set; }
    }
}