using AutoMapper;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;
using EmployeeHub.Repository;
using EmployeeHub.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using RLIMS.Services.ChatService;
using EmployeeHub.Hubs;

namespace EmployeeHub.Services.ChatServices
{
    public class ChatService : IChatService
    {
        private readonly IGenericRepository<Chat> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(
            IGenericRepository<Chat> repository, 
            IMapper mapper, 
            ILogger<ChatService> logger, 
            IHttpContextAccessor httpContextAccessor,
            IHubContext<ChatHub> hubContext)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        public async Task<ChatHistory> SendMessageToUser(ChatHistoryDto chatHistoryDto)
        {
            var senderUserId = ClaimsExtensions.GetUserId(_httpContextAccessor.HttpContext.User);
            
            // Validate receiver exists and is active
            // (Add this validation based on your user repository)

            var chatHistory = _mapper.Map<ChatHistory>(chatHistoryDto);
            chatHistory.Timestamp = DateTime.UtcNow;
            chatHistory.Id = Guid.NewGuid();

            var chat = _repository.FindByConditionAsync(cm => 
                (cm.User1Id == senderUserId && cm.User2Id == chatHistoryDto.ReceiverUserId) ||
                (cm.User1Id == chatHistoryDto.ReceiverUserId && cm.User2Id == senderUserId))
                .Result.SingleOrDefault();

            if (chat == null)
            {
                chat = new Chat
                { 
                    User1Id = senderUserId,
                    User2Id = chatHistoryDto.ReceiverUserId,
                    MessagesJson = JsonConvert.SerializeObject(new[] { chatHistory })
                };
                await _repository.AddAsync(chat);
            }
            else
            {
                var messages = JsonConvert.DeserializeObject<List<ChatHistory>>(chat.MessagesJson) ?? new List<ChatHistory>();
                messages.Add(chatHistory);
                chat.MessagesJson = JsonConvert.SerializeObject(messages);
                await _repository.UpdateAsync(chat);
            }

            // Notify users via SignalR
            await _hubContext.Clients
                .Users(senderUserId.ToString(), chatHistoryDto.ReceiverUserId.ToString())
                .SendAsync("ReceiveMessage", chatHistory);

            return chatHistory;
        }

        public async Task<IEnumerable<ChatHistory>> GetChatHistoryAsync(Guid senderUserId, Guid receiverUserId)
        {
            var chat = await _repository.FindByConditionAsync(cm => 
                (cm.User1Id == senderUserId && cm.User2Id == receiverUserId) ||
                (cm.User1Id == receiverUserId && cm.User2Id == senderUserId));

            if (chat == null || !chat.Any())
            {
                return new List<ChatHistory>();
            }

            var messages = JsonConvert.DeserializeObject<IEnumerable<ChatHistory>>(chat.First().MessagesJson);
            return messages ?? new List<ChatHistory>();
        }

        public async Task<IEnumerable<Chat>> GetChatListAsync(Guid userId)
        {
            var chats = await _repository.FindByConditionAsync(cm => 
                (cm.User1Id == userId || cm.User2Id == userId) && !cm.IsDeleted);
            return chats;
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            var chat = (await _repository.FindByConditionAsync(cm => cm.Id == chatId)).FirstOrDefault();
            if (chat == null) return;

            chat.IsDeleted = true;
            await _repository.UpdateAsync(chat);

            // Notify users about chat deletion
            await _hubContext.Clients
                .Users(chat.User1Id.ToString(), chat.User2Id.ToString())
                .SendAsync("ChatDeleted", chatId);
        }

        public async Task DeleteMessageAsync(Guid chatId, Guid messageId)
        {
            var chat = (await _repository.FindByConditionAsync(cm => cm.Id == chatId)).FirstOrDefault();
            if (chat == null) return;

            var messages = JsonConvert.DeserializeObject<List<ChatHistory>>(chat.MessagesJson) ?? new List<ChatHistory>();
            var message = messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null) return;

            message.IsDeleted = true;
            chat.MessagesJson = JsonConvert.SerializeObject(messages);
            await _repository.UpdateAsync(chat);

            // Notify users about message deletion
            await _hubContext.Clients
                .Users(chat.User1Id.ToString(), chat.User2Id.ToString())
                .SendAsync("MessageDeleted", chatId, messageId);
        }
        
        public async Task<Chat> UpdateMessageAsync(Guid chatId, Guid messageId, string updatedMessage)
        {
            var chat = (await _repository.FindByConditionAsync(cm => cm.Id == chatId)).FirstOrDefault();
            if (chat == null) return null;

            var messages = JsonConvert.DeserializeObject<List<ChatHistory>>(chat.MessagesJson) ?? new List<ChatHistory>();
            var message = messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null) return null;

            // Save message history
            var messageHistory = string.IsNullOrWhiteSpace(message.History)
                ? new List<ChatHistory>()
                : JsonConvert.DeserializeObject<List<ChatHistory>>(message.History) ?? new List<ChatHistory>();
            
            var messageCopy = JsonConvert.DeserializeObject<ChatHistory>(JsonConvert.SerializeObject(message));
            messageHistory.Add(messageCopy);
            
            // Update message
            message.History = JsonConvert.SerializeObject(messageHistory);
            message.IsEdited = true;
            message.EditedTimestamp = DateTime.UtcNow;
            message.Message = updatedMessage;

            chat.MessagesJson = JsonConvert.SerializeObject(messages);
            await _repository.UpdateAsync(chat);

            // Notify users about message update
            await _hubContext.Clients
                .Users(chat.User1Id.ToString(), chat.User2Id.ToString())
                .SendAsync("MessageUpdated", chatId, messageId, updatedMessage);

            return chat;
        }

        public async Task MarkMessageAsRead(Guid chatId, Guid messageId, Guid userId)
        {
            var chat = (await _repository.FindByConditionAsync(cm => cm.Id == chatId)).FirstOrDefault();
            if (chat == null) return;

            var messages = JsonConvert.DeserializeObject<List<ChatHistory>>(chat.MessagesJson) ?? new List<ChatHistory>();
            var message = messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null) return;

            // Implement read status logic here
            // You might want to add a ReadBy property to ChatHistory

            await _repository.UpdateAsync(chat);

            // Notify sender that message was read
            // await _hubContext.Clients
            //     .User(message.SenderId.ToString())
            //     .SendAsync("MessageRead", chatId, messageId, userId);
        }
    }
}