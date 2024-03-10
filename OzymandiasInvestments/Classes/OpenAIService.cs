using Azure.AI.OpenAI;

namespace OzymandiasInvestments.Classes
{
    public class OpenAIService
    {
        private OpenAIClient _client;

        public OpenAIService(string apiKey)
        {
            _client = new OpenAIClient(apiKey);
        }

        public async Task<string> GetResponseForUserInput(string userInput)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                    {
                        new ChatRequestUserMessage(userInput),
                    }
            };
            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);


            return response.Value.Choices.FirstOrDefault()?.Message.Content.ToString().Trim();
        }
    }
}
