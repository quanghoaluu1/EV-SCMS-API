using System.Security.Claims;
using Chat.Application.Interfaces;
using Chat.Domain.DTOs;
using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(
        IChatService chatService,
        ILogger<ConversationsController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    // POST: api/conversations
    [HttpPost]
    public async Task<ActionResult<Conversation>> CreateConversation([FromBody] CreateConversationRequest request)
    {
        try
        {
            var conversation = await _chatService.CreateConversationAsync(request);
            return CreatedAtAction(nameof(GetConversation), new { id = conversation.Id }, conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/conversations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Conversation>> GetConversation(string id)
    {
        try
        {
            var conversation = await _chatService.GetConversationAsync(id);
            
            if (conversation == null)
                return NotFound();

            return Ok(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/conversations/my
    [HttpGet("my")]
    public async Task<ActionResult<List<Conversation>>> GetMyConversations()
    {
        try
        {
            var userId = GetUserId();
            var userType = GetUserType();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversations = await _chatService.GetUserConversationsAsync(userId, userType);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user conversations");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/conversations/{id}/messages
    [HttpGet("{id}/messages")]
    public async Task<ActionResult<List<ChatMessage>>> GetConversationMessages(
        string id, 
        [FromQuery] int skip = 0, 
        [FromQuery] int limit = 50)
    {
        try
        {
            var messages = await _chatService.GetConversationMessagesAsync(id, skip, limit);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation messages");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/conversations/{id}/transfer
    [HttpPost("{id}/transfer")]
    public async Task<ActionResult> TransferToStaff(string id, [FromBody] TransferToStaffRequest request)
    {
        try
        {
            request.ConversationId = id;
            var success = await _chatService.TransferToStaffAsync(request);

            if (success)
                return Ok(new { message = "Conversation transferred successfully" });

            return BadRequest(new { message = "Failed to transfer conversation" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring conversation");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/conversations/{id}/read
    [HttpPost("{id}/read")]
    public async Task<ActionResult> MarkAsRead(string id)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _chatService.MarkMessagesAsReadAsync(id, userId);

            if (success)
                return Ok(new { message = "Messages marked as read" });

            return BadRequest(new { message = "Failed to mark messages as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read");
            return StatusCode(500, "Internal server error");
        }
    }

    // Helper methods
    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value 
            ?? string.Empty;
    }

    private SenderType GetUserType()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value 
            ?? User.FindFirst("role")?.Value;

        return role?.ToLower() switch
        {
            "staff" => SenderType.Staff,
            "admin" => SenderType.Staff,
            _ => SenderType.Customer
        };
    }
}