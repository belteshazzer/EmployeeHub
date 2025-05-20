using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;


namespace RLIMS.Services.ChatService
{
    public interface IChatService
    {
        Task<ChatHistory> SendMessageToUser(ChatHistoryDto chatHistoryDto);
        Task<IEnumerable<ChatHistory>> GetChatHistoryAsync(Guid senderUserId, Guid receiverUserId);
        Task<IEnumerable<Chat>> GetChatListAsync(Guid userId);
        Task DeleteChatAsync(Guid chatId);
        Task DeleteMessageAsync(Guid chatId, Guid messageId);
        Task<Chat> UpdateMessageAsync(Guid chatId, Guid messageId, string updatedMessage);
    }
}