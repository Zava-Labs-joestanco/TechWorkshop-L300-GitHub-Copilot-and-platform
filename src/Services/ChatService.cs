using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;

namespace ZavaStorefront.Services
{
    public class ChatService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IConfiguration configuration, ILogger<ChatService> logger)
        {
            _logger = logger;

            var endpoint = configuration["AzureAI:Endpoint"]
                ?? throw new InvalidOperationException("AzureAI:Endpoint is not configured.");
            var deploymentName = configuration["AzureAI:DeploymentName"]
                ?? throw new InvalidOperationException("AzureAI:DeploymentName is not configured.");

            var azureClient = new AzureOpenAIClient(
                new Uri(endpoint),
                new DefaultAzureCredential());

            _chatClient = azureClient.GetChatClient(deploymentName);
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            _logger.LogInformation("Sending message to AI model");

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful shopping assistant for Zava Storefront."),
                new UserChatMessage(userMessage)
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);
            return completion.Content[0].Text;
        }
    }
}
