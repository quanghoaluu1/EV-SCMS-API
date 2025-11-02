using System.Security.Claims;
using Chat.Application.Interfaces;
using Chat.Domain.DTOs;
using Chat.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.WebApi.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IPresenceService _presenceService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IChatService chatService,
        IPresenceService presenceService,
        ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _presenceService = presenceService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        var userName = GetUserName();

        if (!string.IsNullOrEmpty(userId))
        {
            await _presenceService.SetUserOnlineAsync(userId, Context.ConnectionId);
            
            // Notify others about online status
            await Clients.Others.SendAsync("UserOnline", new
            {
                UserId = userId,
                UserName = userName,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation($"User {userId} connected with connection {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var userName = GetUserName();

        if (!string.IsNullOrEmpty(userId))
        {
            await _presenceService.SetUserOfflineAsync(userId);
            
            // Notify others about offline status
            await Clients.Others.SendAsync("UserOffline", new
            {
                UserId = userId,
                UserName = userName,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation($"User {userId} disconnected");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Join conversation room
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        
        var userId = GetUserId();
        _logger.LogInformation($"User {userId} joined conversation {conversationId}");

        // Mark messages as read
        if (!string.IsNullOrEmpty(userId))
        {
            await _chatService.MarkMessagesAsReadAsync(conversationId, userId);
        }
    }

    // Leave conversation room
    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        
        var userId = GetUserId();
        _logger.LogInformation($"User {userId} left conversation {conversationId}");
    }

    // Send message
    public async Task SendMessage(SendMessageRequest request)
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();
            var userType = GetUserType();

            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            // Send message through service
            var message = await _chatService.SendMessageAsync(userId, userName, userType, request);

            // Broadcast to conversation group
            await Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", new
            {
                message.Id,
                message.ConversationId,
                message.SenderId,
                message.SenderName,
                message.SenderType,
                message.Content,
                message.Timestamp
            });

            _logger.LogInformation($"Message sent: {message.Id} in conversation {request.ConversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    // Typing indicator
    public async Task Typing(string conversationId, bool isTyping)
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();

            if (string.IsNullOrEmpty(userId)) return;

            await _presenceService.SetTypingAsync(conversationId, userId, isTyping);

            // Notify others in the conversation
            await Clients.OthersInGroup(conversationId).SendAsync("UserTyping", new
            {
                ConversationId = conversationId,
                UserId = userId,
                UserName = userName,
                IsTyping = isTyping
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling typing indicator");
        }
    }

    // Mark message as read
    public async Task MarkAsRead(string conversationId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return;

            await _chatService.MarkMessagesAsReadAsync(conversationId, userId);

            // Notify sender about read receipt
            await Clients.Group(conversationId).SendAsync("MessagesRead", new
            {
                ConversationId = conversationId,
                ReadBy = userId,
                ReadAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read");
        }
    }

    // Transfer to staff (customer can request)
    public async Task RequestTransferToStaff(string conversationId, string reason)
    {
        try
        {
            var request = new TransferToStaffRequest
            {
                ConversationId = conversationId,
                Reason = reason
            };

            var success = await _chatService.TransferToStaffAsync(request);

            if (success)
            {
                await Clients.Group(conversationId).SendAsync("ConversationTransferred", new
                {
                    ConversationId = conversationId,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring conversation");
            await Clients.Caller.SendAsync("Error", "Failed to transfer conversation");
        }
    }

    // Get online users
    public async Task GetOnlineUsers()
    {
        try
        {
            var onlineUsers = await _presenceService.GetOnlineUsersAsync();
            await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting online users");
        }
    }

    // Helper methods
    private string GetUserId()
    {
        return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? Context.User?.FindFirst("sub")?.Value 
            ?? string.Empty;
    }

    private string GetUserName()
    {
        return Context.User?.FindFirst(ClaimTypes.Name)?.Value 
            ?? Context.User?.FindFirst("name")?.Value 
            ?? "Unknown User";
    }

    private SenderType GetUserType()
    {
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value 
            ?? Context.User?.FindFirst("role")?.Value;

        return role?.ToLower() switch
        {
            "staff" => SenderType.Staff,
            "admin" => SenderType.Staff,
            _ => SenderType.Customer
        };
    }
}