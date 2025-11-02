using Chat.Application.Interfaces;
using Chat.Domain.DTOs;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Chat.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IAiChatService _aiBotService;
    // private readonly INotificationService _notificationService;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IChatRepository chatRepository,
        IConversationRepository conversationRepository,
        IAiChatService aiBotService,
        ILogger<ChatService> logger)
    {
        _chatRepository = chatRepository;
        _conversationRepository = conversationRepository;
        _aiBotService = aiBotService;
        // _notificationService = notificationService;
        _logger = logger;
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
        
        if (senderType == SenderType.Customer && conversation?.Type == SenderType.AiBot)
        {
            // Check xem có cần transfer sang human không
            var shouldTransfer = await _aiBotService.ShouldTransferToHumanAsync(request.Content);
            
            if (shouldTransfer)
            {
                // Tự động transfer
                var transferRequest = new TransferToStaffRequest
                {
                    ConversationId = request.ConversationId,
                    Reason = "AI Bot detected complex query"
                };
                await TransferToStaffAsync(transferRequest);
            }
            else
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

    public async Task<bool> TransferToStaffAsync(TransferToStaffRequest request)
    {
        // TODO: Implement staff assignment logic (round-robin, least busy, etc.)
        // Hiện tại assign random hoặc theo request.StaffId
        
        var conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);
        if (conversation == null) return false;

        var staffId = request.StaffId ?? "auto-assigned-staff"; // TODO: Get available staff
        var staffName = "Support Staff"; // TODO: Get staff name from User service

        await _conversationRepository.AssignStaffAsync(request.ConversationId, staffId, staffName);

        // Tạo system message
        var systemMessage = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId = "system",
            SenderName = "System",
            SenderType = SenderType.AiBot,
            Content = $"Cuộc hội thoại đã được chuyển đến {staffName}. Lý do: {request.Reason}",
            Timestamp = DateTime.UtcNow
        };

        await _chatRepository.CreateMessageAsync(systemMessage);

        // Notify staff
        // await _notificationService.SendTransferNotificationAsync(staffId, request.ConversationId);

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
}