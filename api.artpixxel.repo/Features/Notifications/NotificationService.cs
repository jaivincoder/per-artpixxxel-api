

using api.artpixxel.data.Features.Notifications;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Extensions;
using api.artpixxel.service.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Notifications
{
    public class NotificationService : INotificationService
    {

        private readonly ArtPixxelContext _context;
        private readonly ICurrentUserService _currentUserService;
        public NotificationService(ArtPixxelContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<NewNotificationResponse> NewNotifications()
        {
            try
            {
                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {
                    return new NewNotificationResponse
                    {
                        Notifications = decimal.Round(await _context.UserNotifications.Where(e => e.UserId == _currentUserService.GetUserId() && (e.ReadStatus == ReadStatus.Unread)).CountAsync(), 0, MidpointRounding.AwayFromZero),
                        Orders = decimal.Round(await _context.Orders.Where(e => e.Seen == false).CountAsync(), 0, MidpointRounding.AwayFromZero)
                    };

                }


                return new NewNotificationResponse
                {
                    Notifications = 0,
                    Orders = 0
                };
            }
            catch (Exception)
            {

                return new NewNotificationResponse
                {
                    Notifications = 0,
                    Orders = 0
                };
            }
        }

        public async Task<NotificationResponse> Notifications(NotificationRequest request)
        {
            try
            {
                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {
                    SqlParameter[] myparm = new SqlParameter[1];
                    myparm[0] = new SqlParameter("@request", request);

                    List<UserNotification> notifications = await _context.UserNotifications.Where(e => e.UserId == _currentUserService.GetUserId())
                        .Include(nn => nn.Notification)
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(0)
                        .Take(request.PageSize).ToListAsync();


               
                    


                    if (notifications.Any())
                    {

                        //change it to read
                        List<UserNotification> unreadNotifications = notifications.Where(e => e.ReadStatus == ReadStatus.Unread).ToList();

                        if (unreadNotifications.Any())
                        {
                            foreach(UserNotification userNotification in unreadNotifications)
                            {
                                userNotification.ReadStatus = ReadStatus.Read;
                            }

                            _context.UserNotifications.UpdateRange(unreadNotifications);
                            await _context.SaveChangesAsync();
                        }
                   



                        return new NotificationResponse
                        {
                          Notifications = notifications
                         .Select(n => new NotificationResponseModel
                         {
                           Id = n.NotificationId,
                           Message = n.Notification.Message,
                           Title = string.IsNullOrEmpty(n.Notification.Title) ? null : n.Notification.Title,
                           Time = n.Notification.CreatedOn.GetElapsedTime()

                          }).ToList()
                        };
                    }

                   
                }

                return new NotificationResponse 
                { 
                    Notifications = new List<NotificationResponseModel>()
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<NotificationResponseModel>> Notifications()
        {
            try
            {
                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {


                    return await _context.UserNotifications.Where(e => e.UserId == _currentUserService.GetUserId())
                        .Include(nn => nn.Notification).ThenInclude(s => s.Subject)
                        .OrderByDescending(e => e.CreatedOn)
                        .Skip(0)
                        .Take(Constants.MaxNotification)
                        .Select(n => new NotificationResponseModel
                        {
                            Id = n.NotificationId,
                            Message = n.Notification.Message,
                            Title = string.IsNullOrEmpty(n.Notification.Title) ? null : n.Notification.Title,
                            SubjectPhoto = string.IsNullOrEmpty(n.Notification.SubjectId) ? AssetDefault.DefaultUserImage : string.IsNullOrEmpty(n.Notification.Subject.PassportURL)
                            ? AssetDefault.DefaultUserImage : n.Notification.Subject.PassportURL.Base64FromImage().Result,
                            Time = n.Notification.CreatedOn.GetElapsedTime()

                        }).ToListAsync();
                   
                }

                return new List<NotificationResponseModel>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendNotification(NotificationModel request)
        {
            try
            {
                if (request.Recipients.Any())
                {
                   NotificationStatus status =  await SendNotification(request.NotificationRequest);
                    if((!string.IsNullOrEmpty(status.NotificationId)) && (status.Sent))
                    {
                        List<UserNotification> userNotifications = new();
                        
                        foreach(var recipient in request.Recipients)
                        {
                            UserNotification userNotification = new()
                            {
                                NotificationId = status.NotificationId,
                                ReadStatus = ReadStatus.Unread,
                                UserId = recipient
                            };

                            userNotifications.Add(userNotification);
                        }


                        if (userNotifications.Any())
                        {
                            _context.UserNotifications.AddRange(userNotifications);
                            await _context.SaveChangesAsync();
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        private async Task<NotificationStatus> SendNotification(NotificationContent request)
        {
            try
            {
                Notification notification = new()
                {
                    AccessType = request.AccessType,
                    NotificationPriority = request.NotificationPriority,
                    Title = request.Title,
                    SubjectId = string.IsNullOrEmpty(request.SubjectId) ? null : _context.Users.Find(request.SubjectId).Id,
                    Message = request.Message,
                };


                _context.Notifications.Add(notification);
                int saveResult = await _context.SaveChangesAsync();

                if(saveResult > 0)
                {
                    return new NotificationStatus
                    {
                        NotificationId = notification.Id,
                        Sent = true
                    };
                }

                return new NotificationStatus
                {
                    NotificationId = string.Empty,
                    Sent= false
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
