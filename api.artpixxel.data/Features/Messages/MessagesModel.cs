

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Messages
{
  public  class ChatMessage
    {
      public string Id { get; set; }
      public string Subject { get; set; }
      public string Content { get; set; }
      public string SenderId { get; set; }
      public string SenderName { get; set; }
      public string ReceiverId { get; set; }
      public bool Read { get; set; }
      public string Time { get; set; }
      public bool Self { get; set; }
      public bool Delivered { get; set; }
      public ChatAttachment Attachment { get; set; }

    }

    public class ChatAttachment
    {
      public string Title { get; set; }
      public string Attachment { get; set; }
    }


    public class MessageSendStatus
    {
        public string Message { get; set; }
        public bool Succeeded { get; set; }
    }
    public class ChatBodyAttachment
    {
        public string AttachmentTitle { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentRelPath { get; set; }
        public string AttachmentAbsPath { get; set; }
        public FileType AttachmentFileType { get; set; }
    }

    public class ChatBody 
    {
        public string Message { get; set; }
        public ChatBodyAttachment Attachment { get; set; }
    }

    public class NotificationChat : ChatBody
    {
        public string UserId { get; set; }
    }

    public class ChatResponse
    {
        public List<ChatMessage> Messages { get; set; }
        public string PartnerActivity { get; set; }
    }

    public class NewChatResponse : ChatResponse
    {
        public List<ChatMessage> OldMessages { get; set; }
    }

    public class ChatActivity : ChatRequest
    {
        public string Activity { get; set; }
    }

    public class ChatMate
    {

        public string ReceiverId { get; set; }
        
    }

    public class ChatRequest 
    {
     
      public string ReceiverId { get; set; }
      public Pagination Pagination { get; set; }
      public List<string> NewReceived { get; set; }
    }

    public class ChatMessageRequest : ChatRequest
    {
       public string Message { get; set; }
       public string Subject { get; set; }
    }


    public class ChatStatus
    {
        public bool Sent { get; set; }
        public string MessageId { get; set; }
    }


    public class ChatMessageDelete
    {
       public string ReceiverId { get; set; }
       public string SenderId { get; set; }
       public string MessageId { get; set; }
       public ChatRequest Request { get; set; }
    }
}
