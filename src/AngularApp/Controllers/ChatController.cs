using Application.Chats.Dtos;
using Application.Chats.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AngularApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] List<string> participants)
        {
            var chatId = await _chatService.CreateChatAsync(participants);

            return Ok(chatId);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyChats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var chats = await _chatService.GetUserChatsAsync(userId);

            return Ok(chats);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(Guid chatId, int page = 1)
        {
            var messages = await _chatService.GetChatMessagesAsync(chatId, page);

            return Ok(messages);
        }

        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] SendMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var message = await _chatService.SendMessageAsync(chatId, userId, request.Content);

            return Ok(message);
        }

        [HttpPost("{chatId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _chatService.MarkMessagesAsReadAsync(chatId, userId);

            return Ok();
        }
    }
}
