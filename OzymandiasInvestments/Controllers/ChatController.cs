using Microsoft.AspNetCore.Mvc;
using OzymandiasInvestments.Classes;
using OzymandiasInvestments.Models.SolutionModels;

namespace OzymandiasInvestments.Controllers
{
    public class ChatController : Controller
    {
        private readonly OpenAIService _chatService;

        public ChatController(OpenAIService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public IActionResult OzymandiasGPT()
        {
            return View("OzymandiasGPT");
        }
        [HttpPost]
        public async Task<IActionResult> OzymandiasGPT([FromBody] ChatInputModel input)
        {
            if (input == null || string.IsNullOrEmpty(input.userInput))
            {
                return Json(new { success = false, message = "Input is null or empty." });
            }
            try
            {
                var response = await _chatService.GetResponseForUserInput(input.userInput);
                return Json(new { success = true, response = response });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
