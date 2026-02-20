

using api.artpixxel.data.Features.Messages;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using System;
using Microsoft.AspNetCore.Identity;
using api.artpixxel.data.Features.Notifications;
using api.artpixxel.data.Features.Common;
using api.artpixxel.repo.Extensions;

namespace api.artpixxel.repo.Features.Messages
{
    public class MessageService : IMessageService
    {
        private readonly UserManager<User> _userManager;
        private readonly ArtPixxelContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        public MessageService(ArtPixxelContext context, UserManager<User> userManager, 
            ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _notificationService = notificationService;
        }
        public async Task<ChatResponse> Conversation(ChatRequest request, User _passedUser = null)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if(!string.IsNullOrEmpty(@request.ReceiverId))
                {
                    if (await _context.Users.AnyAsync(e => e.Id == @request.ReceiverId))
                    {

                       
                        if (_passedUser == null)
                        {

                            _passedUser = await _userManager.GetUserAsync(_currentUserService.GetUser());
                            if(_passedUser.CurrentChat != @request.ReceiverId)
                            {
                                _passedUser.CurrentChat = @request.ReceiverId;
                                await _userManager.UpdateAsync(_passedUser);
                            }

                         List<UserMessage> userMessages = await _context.UserMessages
                        .Where(e => (e.UserId == _currentUserService.GetUserId() && e.Message.UserSentMessages.Any(e => e.UserId == @request.ReceiverId))
                        || (e.UserId == @request.ReceiverId && e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))))
                        .Include(e => e.Message)
                        .OrderBy(e => e.CreatedOn)
                        .Skip(request.Pagination.Skip)
                        .Take(request.Pagination.PageSize).ToListAsync();

                        User user = await _context.Users.Where(e => e.Id == request.ReceiverId).FirstOrDefaultAsync();
                        if (userMessages.Any())
                        {
                            List<UserMessage> currentUserMessages = userMessages.Where(e => e.UserId == _currentUserService.GetUserId() && (e.Delivered == false) && (e.ReadStatus == ReadStatus.Unread)).ToList();
                            if (currentUserMessages.Any())
                            {
                                foreach (UserMessage currentUserMessage in currentUserMessages)
                                {
                                    currentUserMessage.Delivered = true;
                                    currentUserMessage.ReadStatus = ReadStatus.Read;
                                }

                                _context.UserMessages.UpdateRange(currentUserMessages);
                                await _context.SaveChangesAsync();
                            }




                            return new ChatResponse
                            {
                                Messages = userMessages.Select(c =>
                            new ChatMessage
                            {
                                Id = c.Message.Id,
                                Content = c.Message.Content,
                                Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                                Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                                Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                                ReceiverId = c.UserId,
                                SenderId = c.UserId == @request.ReceiverId ? _currentUserService.GetUserId() : @request.ReceiverId,
                                Subject = c.Message.Subject,
                                Time = c.Message.CreatedOn.GetElapsedTime(),
                                Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                            }).ToList(),
                                PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                            };

                        }



                        return new ChatResponse
                        {
                            Messages = new List<ChatMessage>(),
                            PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                        };


                       }

                    }
                }

                else
                {
                    User user = await ActiveAdmin();

                    List<UserMessage> userMessages = await _context.UserMessages
                        .Where(e => (e.UserId == _currentUserService.GetUserId() && e.Message.UserSentMessages.Any(e => e.UserId == user.Id))
                        || (e.UserId == user.Id && e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))))
                        .Include(e => e.Message)
                        .OrderBy(e => e.CreatedOn)
                        .Skip(request.Pagination.Skip)
                        .Take(request.Pagination.PageSize)
                        .ToListAsync();

                    if (userMessages.Any())
                    {
                        List<UserMessage> currentUserMessages = userMessages.Where(e => e.UserId == _currentUserService.GetUserId() && (e.Delivered == false) && (e.ReadStatus == ReadStatus.Unread)).ToList();
                        if (currentUserMessages.Any())
                        {
                            foreach (UserMessage currentUserMessage in currentUserMessages)
                            {
                                currentUserMessage.Delivered = true;
                                currentUserMessage.ReadStatus = ReadStatus.Read;
                            }

                            _context.UserMessages.UpdateRange(currentUserMessages);
                            await _context.SaveChangesAsync();
                        }


                        return new ChatResponse
                        {
                            Messages = userMessages.Select(c =>
                        new ChatMessage
                        {
                            Id = c.Message.Id,
                            Content = c.Message.Content,
                            Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                            Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                            Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                            ReceiverId = c.UserId,
                            SenderId = c.UserId == user.Id ? _currentUserService.GetUserId() : user.Id,
                            Subject = c.Message.Subject,
                            Time = c.Message.CreatedOn.GetElapsedTime(),
                            Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                        }).ToList(),
                            PartnerActivity = string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                        };
                    }


                    return new ChatResponse
                    {
                        Messages = new List<ChatMessage>(),
                        PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                    };





                }



                return new ChatResponse
                {
                    PartnerActivity = string.Empty,
                    Messages = new List<ChatMessage>()
                };
               
                
                
            }
            catch (Exception ex)
            {

                return new ChatResponse
                {
                    PartnerActivity = ex.Message,
                    Messages = new List<ChatMessage>()
                };

            }
        }


        private async Task<User> ActiveAdmin()
        {
           

            if (await _context.Users.AnyAsync(e => e.IsAdmin && (e.IsOnline == true)))
            {
              return   await _context.Users.Where(e => e.IsAdmin && (e.IsOnline == true)).FirstOrDefaultAsync();
            }

           
               return  await _context.Users.Where(e => e.IsAdmin).FirstOrDefaultAsync();
            
        }


        private async Task<ChatStatus> SendChat(ChatBody chatBody)
        {
            Message message = new()
            {
                Content = chatBody.Message,
                AttachmentPath = chatBody.Attachment?.AttachmentPath,
                AttachmentTitle =  chatBody.Attachment?.AttachmentTitle,
                AttachmentAbsPath = chatBody.Attachment?.AttachmentAbsPath,
                AttachmentFileType = chatBody.Attachment?.AttachmentFileType,
                AttachmentRelPath = chatBody.Attachment?.AttachmentRelPath
            };

            _context.Messages.Add(message);
            int res = await _context.SaveChangesAsync();
            return new ChatStatus
            {
                Sent = res > 0,
                MessageId = message.Id
            };
        }
        public async Task<ChatResponse> SendMessage(ChatMessageRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (string.IsNullOrEmpty(request.ReceiverId))
                {
                    List<User> users = await _context.Users.Where(e => e.IsAdmin == true).ToListAsync();
                    List<string> offlineAdmins = await _context.Users.Where(e => e.IsAdmin == true && ((e.IsOnline == false) || (e.CurrentChat != _currentUserService.GetUserId()) )).Select(e => e.Id).ToListAsync();
                    if (users.Any())
                    {
                        ChatStatus chatStatus = await SendChat(new ChatBody { Message = request.Message, Attachment = null });
                        if (chatStatus.Sent)
                        {
                            List<UserMessage> userMessages = new();
                            foreach(User user in users)
                            {
                                UserMessage userMessage = new()
                                {
                                    MessageId = chatStatus.MessageId,
                                    UserId = user.Id,
                                    ReadStatus = ReadStatus.Unread,
                                    Delivered = false
                                };

                                userMessages.Add(userMessage);
                            }

                            if (userMessages.Any())
                            {
                                _context.UserMessages.AddRange(userMessages);
                                await _context.SaveChangesAsync();


                                UserSentMessage userSentMessage = new()
                                {
                                    MessageId = chatStatus.MessageId,
                                    UserId = _currentUserService.GetUserId()
                                };


                                _context.UserSentMessages.Add(userSentMessage);
                                await _context.SaveChangesAsync();


                               


                                if (offlineAdmins.Any())
                                {
                                    User user = await _userManager.GetUserAsync(_currentUserService.GetUser());

                                    if (user != null)
                                    {
                                        await _notificationService.SendNotification(
                                       new NotificationModel
                                       {
                                           Recipients = offlineAdmins,
                                           NotificationRequest = new NotificationContent
                                           {
                                               AccessType = AccessType.Specific,
                                               SubjectId = user.Id,
                                               NotificationPriority = NotificationPriority.Default,
                                               Message = user.FullName +" sent a message.",
                                               Title = "New Message"

                                          }
                                       });
                                        // sendNotification
                                    }

                                }



                                return await Conversation(new ChatRequest() { Pagination = @request.Pagination, ReceiverId = @request.ReceiverId }); 
                                   
                                

                            }
                        }
                    }

                }

                else
                {
                    User user = await _context.Users.Where(e => e.Id == request.ReceiverId).FirstOrDefaultAsync();
                    if (user != null)
                    {

                        List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(a => a.Id).ToListAsync();
                        if (admins.Any())
                        {
                            ChatStatus chatStatus = await SendChat(new ChatBody { Message = request.Message, Attachment = null });
                            if (chatStatus.Sent)
                            {


                                UserMessage userMessage = new()
                                {
                                    MessageId = chatStatus.MessageId,
                                    UserId = user.Id,
                                    ReadStatus = ReadStatus.Unread,
                                    Delivered = false
                                };

                                _context.UserMessages.Add(userMessage);
                                await _context.SaveChangesAsync();

                                List<UserSentMessage> userSentMessages = new();

                                foreach(string admin in admins)
                                {
                                    UserSentMessage userSentMessage = new()
                                    {
                                        MessageId = chatStatus.MessageId,
                                        UserId = admin
                                    };

                                    userSentMessages.Add(userSentMessage);
                                }

                                _context.UserSentMessages.AddRange(userSentMessages);
                                await _context.SaveChangesAsync();

                               


                                return await Conversation(new ChatRequest() { Pagination = @request.Pagination, ReceiverId = @request.ReceiverId });
                            }

                        }

                      
                    }
                }

               


                

              


                
                return new ChatResponse
                {
                    Messages = new List<ChatMessage>(),
                    PartnerActivity =  string.Empty
                };


            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<MessageSendStatus> SendNotificationMessage(NotificationChat notificationChat)
        {
            try
            {
                if (await _context.Users.AnyAsync(e => e.Id == notificationChat.UserId))
                {

                    List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(a => a.Id).ToListAsync();
                    if (admins.Any())
                    {
                        ChatStatus chatStatus = await SendChat(new ChatBody { Message = notificationChat.Message, Attachment = notificationChat.Attachment });
                        if (chatStatus.Sent)
                        {


                            UserMessage userMessage = new()
                            {
                                MessageId = chatStatus.MessageId,
                                UserId = notificationChat.UserId,
                                ReadStatus = ReadStatus.Unread,
                                Delivered = false
                            };

                            _context.UserMessages.Add(userMessage);
                            await _context.SaveChangesAsync();

                            List<UserSentMessage> userSentMessages = new();

                            foreach (string admin in admins)
                            {
                                UserSentMessage userSentMessage = new()
                                {
                                    MessageId = chatStatus.MessageId,
                                    UserId = admin
                                };

                                userSentMessages.Add(userSentMessage);
                            }

                            _context.UserSentMessages.AddRange(userSentMessages);
                            await _context.SaveChangesAsync();



                            return new MessageSendStatus
                            {
                                Message = "Message Sent",
                                Succeeded = true
                            };

                           
                        }

                    }


                }

                return new MessageSendStatus
                {
                    Message = "Recipient user not found",
                    Succeeded = false
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
     
        private async Task<bool> AnyNewMessages(ChatRequest request)
        {
            if (request.NewReceived.Any())
            {
                return await _context.UserMessages
                        .AnyAsync(e => ((e.UserId == _currentUserService.GetUserId()) && (e.Message.UserSentMessages.Any(e => e.UserId == @request.ReceiverId)) 
                        && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false))
                        || ((e.UserId == @request.ReceiverId) && (e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))) 
                        && ((e.ReadStatus == ReadStatus.Unread) && (e.Delivered == false) && (!request.NewReceived.Contains(e.MessageId)))));
            }

            return await _context.UserMessages
                        .AnyAsync(e => ((e.UserId == _currentUserService.GetUserId()) && (e.Message.UserSentMessages.Any(e => e.UserId == @request.ReceiverId)) && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false))
                        || ((e.UserId == @request.ReceiverId) && (e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))) && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false)));

        }
        public async Task<NewChatResponse> NewConversation(ChatRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (!string.IsNullOrEmpty(request.ReceiverId))
                {
                    if (await _context.Users.AnyAsync(e => e.Id == request.ReceiverId))
                    {
                        User user = await _context.Users.Where(e => e.Id == request.ReceiverId).FirstOrDefaultAsync();
                        if (user != null)
                        {
                            List<UserMessage> userMessages = new();

                            if (await AnyNewMessages(@request))
                            {

                                userMessages = await _context.UserMessages
                               .Where(e => ((e.UserId == _currentUserService.GetUserId()) && (e.Message.UserSentMessages.Any(e => e.UserId == @request.ReceiverId)) && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false))
                               || ((e.UserId == @request.ReceiverId) && (e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))) && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false)))
                               .Include(e => e.Message)
                               .OrderBy(e => e.CreatedOn)
                               .ToListAsync();
                                if (userMessages.Any())
                                {
                                    List<UserMessage> currentUserMessages = userMessages.Where(e => e.UserId == _currentUserService.GetUserId() && (e.Delivered == false) && (e.ReadStatus == ReadStatus.Unread)).ToList();
                                    if (currentUserMessages.Any())
                                    {
                                        foreach (UserMessage currentUserMessage in currentUserMessages)
                                        {
                                            currentUserMessage.Delivered = true;
                                            currentUserMessage.ReadStatus = ReadStatus.Read;
                                        }

                                        _context.UserMessages.UpdateRange(currentUserMessages);
                                        await _context.SaveChangesAsync();
                                    }




                                    return new NewChatResponse
                                    {
                                        OldMessages = await RefreshedConversation(request),
                                        Messages = userMessages.Select(c =>
                                    new ChatMessage
                                    {
                                        Id = c.Message.Id,
                                        Content = c.Message.Content,
                                        Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                                    Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                                        Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                                        ReceiverId = c.UserId,
                                        SenderId = c.UserId == @request.ReceiverId ? _currentUserService.GetUserId() : @request.ReceiverId,
                                        Subject = c.Message.Subject,
                                        Time = c.Message.CreatedOn.GetElapsedTime(),
                                        Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                                    }).ToList(),
                                        PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                                    };

                                }
                            }




                            return new NewChatResponse
                            {
                                OldMessages = await RefreshedConversation(request),
                                Messages = new List<ChatMessage>(),
                                PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                            };

                        }
                    }
                }

                else
                {
                    User user = await ActiveAdmin();
                    if (user != null)
                    {
                        @request.ReceiverId = user.Id;


                        bool newMessages = await AnyNewMessages(@request);

                    if (newMessages)
                    {
                        List<UserMessage> userMessages = await _context.UserMessages
                       .Where(e => ((e.UserId == _currentUserService.GetUserId()) && (e.Message.UserSentMessages.Any(e => e.UserId == user.Id)) && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false))
                       || ((e.UserId == user.Id) && (e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))) && (e.ReadStatus == ReadStatus.Unread && e.Delivered == false)))
                       .Include(e => e.Message)
                       .OrderBy(e => e.CreatedOn)
                       .ToListAsync();

                        if (userMessages.Any())
                        {
                            List<UserMessage> currentUserMessages = userMessages.Where(e => e.UserId == _currentUserService.GetUserId() && (e.Delivered == false) && (e.ReadStatus == ReadStatus.Unread)).ToList();
                            if (currentUserMessages.Any())
                            {
                                foreach (UserMessage currentUserMessage in currentUserMessages)
                                {
                                    currentUserMessage.Delivered = true;
                                    currentUserMessage.ReadStatus = ReadStatus.Read;
                                }

                                _context.UserMessages.UpdateRange(currentUserMessages);
                                await _context.SaveChangesAsync();
                            }


                            return new NewChatResponse
                            {
                                OldMessages = await RefreshedConversation(request),
                                Messages = userMessages.Select(c =>
                            new ChatMessage
                            {
                                Id = c.Message.Id,
                                Content = c.Message.Content,
                                Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                                Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                                Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                                ReceiverId = c.UserId,
                                SenderId = c.UserId == user.Id ? _currentUserService.GetUserId() : user.Id,
                                Subject = c.Message.Subject,
                                Time = c.Message.CreatedOn.GetElapsedTime(),
                                Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                            }).ToList(),
                                PartnerActivity = string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                            };
                        }
                    }





                    return new NewChatResponse
                    {
                        OldMessages = await RefreshedConversation(request),
                        Messages = new List<ChatMessage>(),
                        PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                    };




                }
            }


                return new NewChatResponse
                {
                    OldMessages = await RefreshedConversation(request),
                    PartnerActivity = string.Empty,
                    Messages = new List<ChatMessage>()
                };



            }
            catch (Exception ex)
            {

                return new NewChatResponse
                {
                    OldMessages = await RefreshedConversation(request),
                    PartnerActivity = ex.Message,
                    Messages = new List<ChatMessage>()
                };

            }
        }

        public async Task<ChatResponse> UpdateActivity(ChatActivity request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                if(user!= null)
                {
                    user.CurrentActivity = @request.Activity;
                   IdentityResult result =  await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return await Conversation(new ChatRequest { ReceiverId = @request.ReceiverId, Pagination = request.Pagination}, user);
                    }
                }

                return new ChatResponse
                {
                    PartnerActivity = string.Empty,
                    Messages = new List<ChatMessage>()
                };

            }
            catch (Exception ex)
            {

                return new ChatResponse
                {
                    PartnerActivity = ex.Message,
                    Messages = new List<ChatMessage>()
                };
            }
        }



        private async Task<List<ChatMessage>> RefreshedConversation(ChatRequest request)
        {
            try
            {
               


                if (!string.IsNullOrEmpty(request.ReceiverId))
                {
                    if (await _context.Users.AnyAsync(e => e.Id == request.ReceiverId))
                    {

                        List<UserMessage> userMessages = await _context.UserMessages
                        .Where(e => (e.UserId == _currentUserService.GetUserId() && e.Message.UserSentMessages.Any(e => e.UserId == request.ReceiverId))
                        || (e.UserId == @request.ReceiverId && e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))))
                        .Include(e => e.Message)
                        .OrderBy(e => e.CreatedOn)
                        .Skip(request.Pagination.Skip)
                        .Take(request.Pagination.PageSize).ToListAsync();
                        if (userMessages.Any())
                        {



                            User user = await _context.Users.Where(e => e.Id == request.ReceiverId).FirstOrDefaultAsync();

                            return userMessages.Select(c =>
                            new ChatMessage
                            {
                                Id = c.Message.Id,
                                Content = c.Message.Content,
                                Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                                Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                                Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                                ReceiverId = c.UserId,
                                SenderId = c.UserId == @request.ReceiverId ? _currentUserService.GetUserId() : @request.ReceiverId,
                                Subject = c.Message.Subject,
                                Time = c.Message.CreatedOn.GetElapsedTime(),
                                Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                            }).ToList();
                            

                        }




                    }
                }

                else
                {
                    User user = await ActiveAdmin();

                    List<UserMessage> userMessages = await _context.UserMessages
                        .Where(e => (e.UserId == _currentUserService.GetUserId() && e.Message.UserSentMessages.Any(e => e.UserId == user.Id))
                        || (e.UserId == user.Id && e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))))
                        .Include(e => e.Message)
                        .OrderBy(e => e.CreatedOn)
                        .Skip(request.Pagination.Skip)
                        .Take(request.Pagination.PageSize)
                        .ToListAsync();

                    if (userMessages.Any())
                    {



                        return userMessages.Select(c =>
                        new ChatMessage
                        {
                            Id = c.Message.Id,
                            Content = c.Message.Content,
                            Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                            Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                            Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                            ReceiverId = c.UserId,
                            SenderId = c.UserId == user.Id ? _currentUserService.GetUserId() : user.Id,
                            Subject = c.Message.Subject,
                            Time = c.Message.CreatedOn.GetElapsedTime(),
                            Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                        }).ToList();
                           
                    }







                }



                return new List<ChatMessage>();



            }
            catch (Exception)
            {

                return new List<ChatMessage>();

            }
        }
        public async Task<ChatResponse> OlderConversation(ChatRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (!string.IsNullOrEmpty(request.ReceiverId))
                {
                    if (await _context.Users.AnyAsync(e => e.Id == request.ReceiverId))
                    {

                        List<UserMessage> userMessages = await _context.UserMessages
                        .Where(e => (e.UserId == _currentUserService.GetUserId() && e.Message.UserSentMessages.Any(e => e.UserId == @request.ReceiverId))
                        || (e.UserId == @request.ReceiverId && e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))))
                        .Include(e => e.Message)
                        .OrderBy(e => e.CreatedOn)
                        .Skip(request.Pagination.Skip)
                        .Take(request.Pagination.PageSize).ToListAsync();
                        if (userMessages.Any())
                        {
                            


                            User user = await _context.Users.Where(e => e.Id == request.ReceiverId).FirstOrDefaultAsync();

                            return new ChatResponse
                            {
                                Messages = userMessages.Select(c =>
                            new ChatMessage
                            {
                                Id = c.Message.Id,
                                Content = c.Message.Content,
                                Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                                Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                                Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                                ReceiverId = c.UserId,
                                SenderId = c.UserId == @request.ReceiverId ? _currentUserService.GetUserId() : @request.ReceiverId,
                                Subject = c.Message.Subject,
                                Time = c.Message.CreatedOn.GetElapsedTime(),
                                Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                            }).ToList(),
                                PartnerActivity = user == null ? string.Empty : string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                            };

                        }




                    }
                }

                else
                {
                    User user = await ActiveAdmin();

                    List<UserMessage> userMessages = await _context.UserMessages
                        .Where(e => (e.UserId == _currentUserService.GetUserId() && e.Message.UserSentMessages.Any(e => e.UserId == user.Id))
                        || (e.UserId == user.Id && e.Message.UserSentMessages.Any(e => e.UserId == _currentUserService.GetUserId() && (e.SenderDeleted == false))))
                        .Include(e => e.Message)
                        .OrderBy(e => e.CreatedOn)
                        .Skip(request.Pagination.Skip)
                        .Take(request.Pagination.PageSize)
                        .ToListAsync();

                    if (userMessages.Any())
                    {
                        


                        return new ChatResponse
                        {
                            Messages = userMessages.Select(c =>
                        new ChatMessage
                        {
                            Id = c.Message.Id,
                            Content = c.Message.Content,
                            Self = c.UserId == _currentUserService.GetUserId() ? false : true, // message sent to me?
                            Read = c.UserId == _currentUserService.GetUserId() ? true : c.ReadStatus == ReadStatus.Read,
                            ReceiverId = c.UserId,
                            SenderId = c.UserId == user.Id ? _currentUserService.GetUserId() : user.Id,
                            Delivered = c.UserId == _currentUserService.GetUserId() ? true : c.Delivered,
                            Subject = c.Message.Subject,
                            Time =  c.Message.CreatedOn.GetElapsedTime(),
                            Attachment = string.IsNullOrEmpty(c.Message.AttachmentPath) ? null : new ChatAttachment { Title = c.Message.AttachmentTitle, Attachment = c.Message.AttachmentAbsPath }
                        }).ToList(),
                            PartnerActivity = string.IsNullOrEmpty(user.CurrentActivity) ? string.Empty : user.CurrentActivity
                        };
                    }







                }



                return new ChatResponse
                {
                    PartnerActivity = string.Empty,
                    Messages = new List<ChatMessage>()
                };



            }
            catch (Exception ex)
            {

                return new ChatResponse
                {
                    PartnerActivity = ex.Message,
                    Messages = new List<ChatMessage>()
                };

            }
        }

        public async Task<ChatResponse> DeleteForOne(ChatMessageDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(@request.SenderId == _currentUserService.GetUserId()) // message by current user
                {

                    User currentUser = await _userManager.GetUserAsync(_currentUserService.GetUser());
                    if(currentUser != null)
                    {
                        if (currentUser.IsAdmin)
                        {
                            List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                            if (admins.Any())
                            {
                                List<UserSentMessage> userSentMessages = await _context.UserSentMessages.Where(e => admins.Contains(e.UserId) && (e.MessageId == @request.MessageId)).ToListAsync();
                                if (userSentMessages.Any())
                                {
                                    foreach(UserSentMessage userSentMessage in userSentMessages)
                                    {
                                        userSentMessage.SenderDeleted = true;
                                    }
                                    _context.UserSentMessages.UpdateRange(userSentMessages);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            
                        }

                        else
                        {
                            UserSentMessage userSentMessage = await _context.UserSentMessages.Where(e => e.UserId == @request.SenderId && (e.MessageId == @request.MessageId)).FirstOrDefaultAsync();
                            if (userSentMessage != null)
                            {
                                userSentMessage.SenderDeleted = true;
                                _context.UserSentMessages.Update(userSentMessage);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                   
                }

                else
                {
                    if(@request.ReceiverId == _currentUserService.GetUserId())
                    {
                        User currentUser = await _userManager.GetUserAsync(_currentUserService.GetUser());
                        if(currentUser != null)
                        {
                            if (currentUser.IsAdmin)
                            {
                                List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                                if (admins.Any())
                                {
                                    List<UserMessage> userMessages = await _context.UserMessages.Where(e => admins.Contains(e.UserId) && (e.MessageId == @request.MessageId)).ToListAsync();
                                    if (userMessages.Any())
                                    {
                                        _context.UserMessages.RemoveRange(userMessages);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            else
                            {
                                UserMessage userMessage = await _context.UserMessages.Where(e => e.UserId == @request.ReceiverId && (e.MessageId == @request.MessageId)).FirstOrDefaultAsync();
                                if (userMessage != null)
                                {
                                    _context.UserMessages.Remove(userMessage);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        
                    }
                }

                return await OlderConversation(request.Request);
            }
            catch (Exception)
            {

                return await OlderConversation(request.Request);
            }
        }

        public async Task<ChatResponse> DeleteForBoth(ChatMessageDelete request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (@request.SenderId == _currentUserService.GetUserId())
                {

                    User currentUser = await _userManager.GetUserAsync(_currentUserService.GetUser());
                    if(currentUser != null)
                    {
                        if (currentUser.IsAdmin)
                        {
                            List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                            if (admins.Any())
                            {
                                List<UserSentMessage> userSentMessages = await _context.UserSentMessages.Where(e => admins.Contains(e.UserId) && (e.MessageId == @request.MessageId)).ToListAsync();
                                if (userSentMessages.Any())
                                {
                                    _context.UserSentMessages.RemoveRange(userSentMessages);
                                    await _context.SaveChangesAsync();
                                }


                                UserMessage userMessage = await _context.UserMessages.Where(e => e.UserId == @request.ReceiverId && (e.MessageId == @request.MessageId)).FirstOrDefaultAsync();
                                if (userMessage != null)
                                {
                                     
                                    _context.UserMessages.Remove(userMessage);
                                    await _context.SaveChangesAsync();
                                }

                            }
                           
                            
                        }

                        else
                        {
                            List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                            if (admins.Any())
                            {
                                List<UserMessage> userMessages = await _context.UserMessages.Where(e => admins.Contains(e.UserId) && (e.MessageId == @request.MessageId) ).ToListAsync();
                                if (userMessages.Any())
                                {
                                     
                                    _context.UserMessages.RemoveRange(userMessages);
                                    await _context.SaveChangesAsync();
                                }

                                UserSentMessage userSentMessage = await _context.UserSentMessages
                                    .Where(e => e.UserId == request.SenderId && (e.MessageId == @request.MessageId))
                                    .FirstOrDefaultAsync();
                                if(userSentMessage != null)
                                {
                                    _context.UserSentMessages.Remove(userSentMessage);
                                    await _context.SaveChangesAsync();
                                }

                               
                            }

                        }



                        Message message = await _context.Messages.FindAsync(@request.MessageId);
                        if (message != null)
                        {
                            _context.Messages.Remove(message);
                            await _context.SaveChangesAsync();
                        }



                    }
                  
                }


                    return await OlderConversation(request.Request);
            }
            catch (Exception)
            {
              
                return await OlderConversation(request.Request);
            }
           

            
        }

        public async Task<BaseBoolStatus> CloseConversation()
        {
            try
            {
                User currentUser = await _userManager.GetUserAsync(_currentUserService.GetUser());
                if(currentUser != null)
                {
                    currentUser.CurrentChat = "";
                    currentUser.CurrentActivity = "";
                    await _userManager.UpdateAsync(currentUser);

                    return new BaseBoolStatus
                    {
                        Status = true
                    };
                }


                return new BaseBoolStatus
                {
                    Status = false
                };


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
