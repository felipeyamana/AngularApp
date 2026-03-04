using Application.Chats.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var userId = User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId)) return BadRequest();

            var chats = await _chatService.GetUserChatsAsync(userId);

            return Ok(chats);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(Guid chatId, int page = 1)
        {
            var messages = await _chatService.GetChatMessagesAsync(chatId, page);

            return Ok(messages);
        }
    }
}
