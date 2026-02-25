using Microsoft.AspNetCore.Mvc;
using ZavaStorefront.Services;

namespace ZavaStorefront.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly ChatService _chatService;

        public ChatController(ILogger<ChatController> logger, ChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string userMessage, string conversationHistory)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                ViewBag.ConversationHistory = conversationHistory ?? string.Empty;
                return View("Index");
            }

            var history = conversationHistory ?? string.Empty;
            history += $"You: {userMessage}\n";

            try
            {
                _logger.LogInformation("User sent chat message");
                var response = await _chatService.GetResponseAsync(userMessage);
                history += $"Assistant: {response}\n\n";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error communicating with AI endpoint");
                history += "Assistant: Sorry, I'm unable to respond right now. Please try again later.\n\n";
            }

            ViewBag.ConversationHistory = history;
            return View("Index");
        }
    }
}
