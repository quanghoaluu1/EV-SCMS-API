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
public class MessagesController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
        IChatService chatService,
        ILogger<MessagesController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    // POST: api/messages
    [HttpPost]
    public async Task<ActionResult<ChatMessage>> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();
            var userType = GetUserType();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = await _chatService.SendMessageAsync(userId, userName, userType, request);
            return CreatedAtAction(nameof(SendMessage), message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
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

    private string GetUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value 
               ?? User.FindFirst("name")?.Value 
               ?? "Unknown User";
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