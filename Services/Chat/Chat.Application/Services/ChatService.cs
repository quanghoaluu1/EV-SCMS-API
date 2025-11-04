using Chat.Application.Interfaces;
using Chat.Domain.DTOs;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Chat.Infrastructure.Interfaces;
using Chat.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IAiChatService _aiBotService;
    // private readonly INotificationService _notificationService;
    private readonly ILogger<ChatService> _logger;
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IStaffHttpGateway _staffHttpGateway;
    public ChatService(
        IChatRepository chatRepository,
        IConversationRepository conversationRepository,
        IAiChatService aiBotService,
        ILogger<ChatService> logger,
        IHubContext<ChatHub> chatHubContext,
        IStaffHttpGateway staffHttpGateway)
    {
        _chatRepository = chatRepository;
        _conversationRepository = conversationRepository;
        _aiBotService = aiBotService;
        // _notificationService = notificationService;
        _logger = logger;
        _chatHubContext = chatHubContext;
        _staffHttpGateway = staffHttpGateway;
    }

    public async Task<Conversation> CreateConversationAsync(CreateConversationRequest request)
    {
        var conversation = new Conversation
        {
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            Type = request.Type,
            Status = ConversationStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _conversationRepository.CreateConversationAsync(conversation);

        // Nếu có initial message, gửi luôn
        if (!string.IsNullOrEmpty(request.InitialMessage))
        {
            var messageRequest = new SendMessageRequest
            {
                ConversationId = conversation.Id,
                Content = request.InitialMessage,
            };

            await SendMessageAsync(request.CustomerId, request.CustomerName, SenderType.Customer, messageRequest);
        }

        return conversation;
    }

    public async Task<ChatMessage> SendMessageAsync(
        string userId, 
        string userName, 
        SenderType senderType, 
        SendMessageRequest request)
    {
        // Tạo message
        var message = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId = userId,
            SenderName = userName,
            SenderType = senderType,
            Content = request.Content,
            Timestamp = DateTime.UtcNow
        };

        // Lưu message vào MongoDB
        await _chatRepository.CreateMessageAsync(message);

        // Update conversation
        await _conversationRepository.UpdateLastMessageAsync(request.ConversationId, request.Content);
        await _conversationRepository.IncrementUnreadCountAsync(request.ConversationId);

        // Nếu là customer message và conversation type là AIBot
        var conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);
        
        if (senderType == SenderType.Customer && conversation?.Type == ConversationType.AiBot)
        {
                // AI Bot response
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var history = await _chatRepository.GetMessagesByConversationIdAsync(request.ConversationId);
                        var aiResponse = await _aiBotService.GenerateResponseAsync(
                            request.ConversationId, 
                            request.Content, 
                            history);

                        var botMessage = new ChatMessage
                        {
                            ConversationId = request.ConversationId,
                            SenderId = "ai-bot",
                            SenderName = "AI Assistant",
                            SenderType = SenderType.AiBot,
                            Content = aiResponse,
                            Timestamp = DateTime.UtcNow
                        };

                        await _chatRepository.CreateMessageAsync(botMessage);
                        await _conversationRepository.UpdateLastMessageAsync(request.ConversationId, aiResponse);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating AI response");
                    }
                });
        }

        // Send notification nếu recipient offline
        // TODO: Check user online status và send notification

        return message;
    }

    public async Task<List<ChatMessage>> GetConversationMessagesAsync(string conversationId, int skip = 0, int limit = 50)
    {
        return await _chatRepository.GetMessagesByConversationIdAsync(conversationId, skip, limit);
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(string userId, SenderType userType)
    {
        if (userType == SenderType.Customer)
        {
            return await _conversationRepository.GetConversationsByCustomerIdAsync(userId);
        }
        else if (userType == SenderType.Staff)
        {
            return await _conversationRepository.GetConversationsByStaffIdAsync(userId);
        }

        return new List<Conversation>();
    }
    
    public async Task<List<Conversation>> GetWaitingConversationsAsync()
    {
        return await _conversationRepository.GetWaitingConversationsAsync();
    }
    
    public async Task<List<Conversation>> GetStaffConversationsAsync(string staffId)
    {
        return await _conversationRepository.GetConversationsByStaffIdAsync(staffId);
    }

    public async Task<bool> RequestTransferToStaffAsync(string conversationId)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(conversationId);
        if (conversation == null) return false;

        // Update status to Waiting
        await _conversationRepository.UpdateStatusAsync(conversationId, ConversationStatus.Waiting);

        // Create system message
        var systemMessage = new ChatMessage
        {
            ConversationId = conversationId,
            SenderId = "system",
            SenderName = "System",
            SenderType = SenderType.System,
            Content = "Cảm ơn bạn đã liên hệ! Chúng tôi đang kết nối bạn với nhân viên tư vấn. Vui lòng chờ trong giây lát hoặc tiếp tục chat với AI Assistant.",
            Timestamp = DateTime.UtcNow
        };

        await _chatRepository.CreateMessageAsync(systemMessage);
        await _conversationRepository.UpdateLastMessageAsync(conversationId, systemMessage.Content);

        _logger.LogInformation($"Conversation {conversationId} requested transfer to staff");

        return true;
    }
    public async Task<bool> TransferToStaffAsync(TransferToStaffRequest request)
    {
        // TODO: Implement staff assignment logic (round-robin, least busy, etc.)
        // Hiện tại assign random hoặc theo request.StaffId
        
        var conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);
        if (conversation == null) return false;

        conversation.Type = ConversationType.Staff;
        conversation.Status = ConversationStatus.Waiting;
        conversation.UpdatedAt = DateTime.UtcNow;
        
        await _conversationRepository.UpdateConversationAsync(conversation);

        // Tạo system message
        var systemMessage = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId = "system",
            SenderName = "System",
            SenderType = SenderType.System,
            Content = $"Cuộc hội thoại đã được chuyển đến nhân viên. Lý do: {request.Reason}",
            Timestamp = DateTime.UtcNow
        };

        await _chatRepository.CreateMessageAsync(systemMessage);

        await _chatHubContext.Clients.Group("Managers").SendAsync("NewPendingConversation",conversation);
        await _chatHubContext.Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", systemMessage);
        return true;
    }
    
    public async Task<bool> AssignStaffToConversationAsync(AssignStaffRequest request)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);
        if (conversation == null) return false;

        // Assign staff and change type to Staff
        await _conversationRepository.AssignStaffAsync(
            request.ConversationId,
            request.StaffId,
            request.StaffName);

        // Update status back to Active
        await _conversationRepository.UpdateStatusAsync(request.ConversationId, ConversationStatus.Active);

        // Create system message
        var systemMessage = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId = "system",
            SenderName = "System",
            SenderType = SenderType.System,
            Content = $"Nhân viên tư vấn {request.StaffName} đã tham gia cuộc hội thoại. Bạn có thể bắt đầu trao đổi!",
            Timestamp = DateTime.UtcNow
        };

        await _chatRepository.CreateMessageAsync(systemMessage);
        await _conversationRepository.UpdateLastMessageAsync(request.ConversationId, systemMessage.Content);
        
        _logger.LogInformation($"Staff {request.StaffName} assigned to conversation {request.ConversationId}");

        return true;
    }

    public async Task<bool> MarkMessagesAsReadAsync(string conversationId, string userId)
    {
        // Get unread messages
        var messages = await _chatRepository.GetMessagesByConversationIdAsync(conversationId);
        var unreadMessages = messages.Where(m => !m.IsRead && m.SenderId != userId);

        foreach (var message in unreadMessages)
        {
            await _chatRepository.MarkAsReadAsync(message.Id);
        }

        // Reset unread count
        await _conversationRepository.ResetUnreadCountAsync(conversationId);

        return true;
    }

    public async Task<Conversation?> GetConversationAsync(string conversationId)
    {
        return await _conversationRepository.GetConversationByIdAsync(conversationId);
    }
    public async Task<List<Conversation>> GetAllConversationsAsync()
    {
        return await _conversationRepository.GetAllConversationAsync();
    }
    public async Task<List<Conversation>> GetMyAssignedConversationsAsync(string userId)
    {
        var staff = await _staffHttpGateway.GetStaffByUserId(Guid.Parse(userId));
        var staffId = staff.Data.StaffId;
        return await _conversationRepository.GetConversationsByStaffIdAsync(staffId);
    }
}