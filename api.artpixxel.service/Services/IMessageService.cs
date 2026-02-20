

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Messages;
using api.artpixxel.data.Models;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
  public  interface IMessageService 
    {
        Task<ChatResponse> Conversation(ChatRequest request, User _passedUser = null);
        Task<BaseBoolStatus>CloseConversation();
        Task<ChatResponse> OlderConversation(ChatRequest request);
        Task<NewChatResponse> NewConversation(ChatRequest request);
        Task<ChatResponse> SendMessage(ChatMessageRequest request);
        Task<MessageSendStatus> SendNotificationMessage(NotificationChat notificationChat);
        Task<ChatResponse> UpdateActivity(ChatActivity request);
        Task<ChatResponse> DeleteForOne(ChatMessageDelete request);
        Task<ChatResponse> DeleteForBoth(ChatMessageDelete request);
    }
}
