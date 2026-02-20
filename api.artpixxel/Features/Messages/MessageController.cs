using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Messages;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Messages
{
   
    public class MessageController : ApiController
    {
        public readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [Route(nameof(Conversation))]
        public async Task<ChatResponse> Conversation(ChatRequest request)
            => await _messageService.Conversation(request);


        [HttpPost]
        [Route(nameof(SendMessage))]
        public async Task<ChatResponse> SendMessage(ChatMessageRequest request)
            => await _messageService.SendMessage(request);

        [HttpPost]
        [Route(nameof(NewConversation))]
        public async Task<ChatResponse> NewConversation(ChatRequest request)
            => await _messageService.NewConversation(request);

        [HttpPost]
        [Route(nameof(UpdateActivity))]
        public async Task<ChatResponse> UpdateActivity(ChatActivity request)
            => await _messageService.UpdateActivity(request);

        [HttpPost]
        [Route(nameof(OlderConversation))]
        public async Task<ChatResponse> OlderConversation(ChatRequest request)
            => await _messageService.OlderConversation(request);

        [HttpPost]
        [Route(nameof(DeleteForOne))]
        public async Task<ChatResponse> DeleteForOne(ChatMessageDelete request)
            => await _messageService.DeleteForOne(request);

        [HttpPost]
        [Route(nameof(DeleteForBoth))]
        public async Task<ChatResponse> DeleteForBoth(ChatMessageDelete request)
            => await _messageService.DeleteForBoth(request);

        [HttpGet]
        [Route(nameof(CloseConversation))]
        public async Task<BaseBoolStatus> CloseConversation()
            => await _messageService.CloseConversation();
    }
}
