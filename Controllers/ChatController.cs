using Microsoft.AspNetCore.Mvc;
using EmployeeHub.Services.ChatServices;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Common.ApiResponse;
using EmployeeHub.Models.Entities;
using RLIMS.Services.ChatService;

namespace EmployeeHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatHistoryDto chatHistoryDto)
        {
            _logger.LogInformation("Sending message from userto user {ReceiverId}", chatHistoryDto.ReceiverUserId);

            var result = await _chatService.SendMessageToUser(chatHistoryDto);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = result,
                Message = "Message sent successfully"
            });
        }

        [HttpGet("chat-history/{senderUserId}/{receiverUserId}")]
        public async Task<IActionResult> GetChatHistory(Guid senderUserId, Guid receiverUserId)
        {
            _logger.LogInformation("Fetching chat history between user {SenderId} and user {ReceiverId}", senderUserId, receiverUserId);

            var result = await _chatService.GetChatHistoryAsync(senderUserId, receiverUserId);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = result,
                Message = "Chat history retrieved successfully"
            });
        }

        [HttpGet("chat-history/{id}")]
        public async Task<IActionResult> GetChatHistory(Guid id)
        {

            var result = await _chatService.GetChatHistoryAsync(id);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = result,
                Message = "Chat history retrieved successfully"
            });
        }

        [HttpGet("chat-list/{userId}")]
        public async Task<IActionResult> GetChatList(Guid userId)
        {
            _logger.LogInformation("Fetching chat list for user {UserId}", userId);

            var result = await _chatService.GetChatListAsync(userId);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = result,
                Message = "Chat list retrieved successfully"
            });
        }

        [HttpDelete("delete-chat")]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            _logger.LogInformation("Deleting chat with ID {ChatId}", chatId);

            await _chatService.DeleteChatAsync(chatId);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = null,
                Message = "Chat deleted successfully"
            });
        }

        [HttpDelete("delete-message")]
        public async Task<IActionResult> DeleteMessage(Guid chatId, Guid messageId)
        {
            _logger.LogInformation("Deleting message with ID {MessageId} in chat {ChatId}", messageId, chatId);

            await _chatService.DeleteMessageAsync(chatId, messageId);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = null,
                Message = "Message deleted successfully"
            });
        }

        [HttpPut("update-message")]
        public async Task<IActionResult> UpdateMessage(Guid chatId, Guid messageId, [FromBody] string updatedMessage)
        {
            _logger.LogInformation("Updating message with ID {MessageId} in chat {ChatId}", messageId, chatId);

            var result = await _chatService.UpdateMessageAsync(chatId, messageId, updatedMessage);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = result,
                Message = "Message updated successfully"
            });
        }

        [HttpPost("mark-message-as-read")]
        public async Task<IActionResult> MarkMessageAsRead(Guid chatId, Guid messageId, Guid userId)
        {
            _logger.LogInformation("Marking message with ID {MessageId} as read in chat {ChatId} by user {UserId}", messageId, chatId, userId);

            await _chatService.MarkMessageAsRead(chatId, messageId, userId);

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Data = null,
                Message = "Message marked as read"
            });
        }
    }
}