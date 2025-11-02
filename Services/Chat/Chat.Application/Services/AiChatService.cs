using System.Text;
using System.Text.Json;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Chat.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chat.Application.Services;

public class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly ILogger<AiChatService> _logger;

    // Keywords để transfer sang human support
    private readonly List<string> _transferKeywords = new()
    {
        "nhân viên", "staff", "người thật", "nói chuyện với người",
        "phức tạp", "không hiểu", "giải quyết không được"
    };

    public AiChatService(
        IOptions<GeminiSettings> settings,
        ILogger<AiChatService> logger)
    {
        _httpClient = new HttpClient();
        _apiKey = settings.Value.ApiKey;
        _model = settings.Value.Model;
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(
        string conversationId, 
        string userMessage, 
        List<ChatMessage> conversationHistory)
    {
        try
        {
            // Build context từ conversation history
            var context = BuildContext(conversationHistory);
            
            // System prompt cho car maintenance service
            var systemPrompt = @"Bạn là trợ lý AI thông minh của hệ thống bảo dưỡng xe. 
Nhiệm vụ của bạn là hỗ trợ khách hàng về:
- Thông tin các gói bảo dưỡng xe
- Lịch hẹn bảo dưỡng
- Giá cả dịch vụ
- Tình trạng xe và khuyến nghị bảo dưỡng
- Các câu hỏi thường gặp về bảo dưỡng xe

Hãy trả lời ngắn gọn, thân thiện và chính xác. Nếu câu hỏi quá phức tạp hoặc cần kiểm tra chi tiết, 
hãy đề xuất khách hàng chuyển sang chat với nhân viên hỗ trợ.";

            var prompt = $"{systemPrompt}\n\nLịch sử hội thoại:\n{context}\n\nKhách hàng: {userMessage}\n\nTrợ lý:";

            var response = await CallGeminiApiAsync(prompt);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI response");
            return "Xin lỗi, tôi đang gặp sự cố. Bạn có muốn tôi chuyển sang chat với nhân viên hỗ trợ không?";
        }
    }

    public async Task<bool> ShouldTransferToHumanAsync(string userMessage)
    {
        try
        {
            var lowerMessage = userMessage.ToLower();
            
            // Check keywords
            if (_transferKeywords.Any(keyword => lowerMessage.Contains(keyword)))
            {
                return true;
            }

            // Use AI to detect intent
            var intent = await GetIntentAsync(userMessage);
            return intent.Contains("transfer") || intent.Contains("human_support");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking transfer to human");
            return false;
        }
    }

    public async Task<string> GetIntentAsync(string userMessage)
    {
        try
        {
            var prompt = $@"Phân loại ý định của khách hàng vào một trong các loại sau:
- booking: Đặt lịch bảo dưỡng
- pricing: Hỏi về giá
- service_info: Hỏi về dịch vụ
- complaint: Phản ánh, khiếu nại
- transfer: Muốn nói chuyện với nhân viên
- general: Câu hỏi chung

Tin nhắn: {userMessage}

Trả lời chỉ một từ khóa phân loại:";

            var intent = await CallGeminiApiAsync(prompt);
            return intent.Trim().ToLower();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting intent");
            return "general";
        }
    }

    private string BuildContext(List<ChatMessage> messages)
    {
        var sb = new StringBuilder();
        var recentMessages = messages.TakeLast(5).ToList();

        foreach (var msg in recentMessages)
        {
            var role = msg.SenderType == SenderType.Customer ? "Khách hàng" : "Trợ lý";
            sb.AppendLine($"{role}: {msg.Content}");
        }

        return sb.ToString();
    }

    private async Task<string> CallGeminiApiAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 1024
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Gemini API error: {responseJson}");
            throw new Exception($"Gemini API returned {response.StatusCode}");
        }

        var result = JsonSerializer.Deserialize<JsonElement>(responseJson);
        
        var textResponse = result
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return textResponse ?? "Xin lỗi, tôi không thể tạo phản hồi lúc này.";
    }
}