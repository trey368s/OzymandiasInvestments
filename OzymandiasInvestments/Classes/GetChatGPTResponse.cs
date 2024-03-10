using Alpaca.Markets;
using Azure;
using Azure.AI.OpenAI;
using OzymandiasInvestments.Models.SolutionModels;
using System.Text.Json;

namespace OzymandiasInvestments.Classes
{
    public class GetChatGPTResponse
    {
        private readonly string _apiKey;
        public GetChatGPTResponse(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<List<NewsArticleModel>> SentimentChatBot(List<INewsArticle> news, string ticker)
        {
            List<NewsArticleModel> newsArticles = new List<NewsArticleModel>();
            OpenAIClient client = new OpenAIClient(_apiKey);
            foreach (INewsArticle article in news)
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-3.5-turbo",
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are a financial assistant that provides sentiment analysis of financial article headlines on a scale of 1 to 100, 1 being horrible, 50 being neutral, and 100 being great. The following headline is realting to the stock ticker " + ticker + ". Only give your sentiment analysis score in JSON format as a string with one key named sentiment having the value in quotes."),
                        new ChatRequestUserMessage(article.Headline),
                    }
                };
                try
                {
                    Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
                    ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
                    NewsArticleModel newsModel = new NewsArticleModel();
                    newsModel.newsArticle = article;
                    SentimentModel sentiment = JsonSerializer.Deserialize<SentimentModel>(json: responseMessage.Content);
                    newsModel.sentiment = sentiment;
                    newsArticles.Add(newsModel);
                }
                catch
                {
                    NewsArticleModel newsModel = new NewsArticleModel();
                    newsModel.newsArticle = article;
                    newsModel.sentiment = null;
                    newsArticles.Add(newsModel);
                }
            }
            return newsArticles;
        }
    }
}