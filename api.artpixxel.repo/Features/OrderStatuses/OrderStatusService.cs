

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Messages;
using api.artpixxel.data.Features.Orders;
using api.artpixxel.data.Features.OrderStatuses;
using api.artpixxel.data.Features.SMTPs;
using api.artpixxel.data.Models;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.OrderStatuses
{
    public class OrderStatusService : IOrderStatusService
    {

        private readonly ArtPixxelContext _context;
        private readonly ISMTPService _sMTPService;
        private readonly IMessageService _messageService;
        public OrderStatusService(ArtPixxelContext context, ISMTPService sMTPService, IMessageService messageService)
        {
            _context = context;
            _sMTPService = sMTPService;
            _messageService = messageService;
        }

        public async Task<OrderStatusHistoryResponse> History(OrderStatusHistoryRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.Orders.AnyAsync(e => e.Id == @request.OrderId))
                {
                    Order order = await _context.Orders.Where(e => e.Id == @request.OrderId)
                            .Include(e => e.Items)
                            .Include(e => e.Histories)
                            .Include(e => e.Customer).ThenInclude(us => us.User)
                            .FirstOrDefaultAsync();
                   
                    if(order != null)
                    {

                       


                        OrderStatus orderStatus = null;
                        List<OrderStatus> orderStatuses = await _context.OrderStatuses.ToListAsync();
                        if (orderStatuses.Any())
                        {
                            if(orderStatuses.Any(e => e.Id == request.OrderStatusId))
                            {
                                orderStatus = orderStatuses.Where(e => e.Id == request.OrderStatusId).FirstOrDefault();
                            }
                        }
                       

                        if (orderStatus != null)
                        {

                            if (string.IsNullOrEmpty(@request.OrderHistoryId)) // create mode
                            {

                                if (order.Histories.Any(e => e.StatusId == request.OrderStatusId))
                                {
                                    return new OrderStatusHistoryResponse
                                    {
                                        Histories = await StatusHistories(@request.OrderId),
                                        Response = new BaseResponse
                                        {
                                            Message = "Status: '"+orderStatus.Label+"' already maintained for order.",
                                            Result = RequestResult.Error,
                                            Succeeded = false,
                                            Title = "Redundant Request"
                                        }
                                    };
                                }


                                else
                                {

                                


                                OrderStatusHistory history = new()
                                {
                                    ColorCode = orderStatus.ColorCode,
                                    Comment = request.OrderStatusComment,
                                    NotificationOption = (NotificationOption)Enum.Parse(typeof(NotificationOption), request.OrderStatusNotification),
                                    Icon = orderStatus.Icon,
                                    Label = orderStatus.Label,
                                    OrderId = order.Id,
                                    StatusId = orderStatus.Id

                                };

                                _context.OrderStatusHistories.Add(history);
                                int saveResult = await _context.SaveChangesAsync();


                                if (saveResult > 0)
                                {

                                    order.StatusId = orderStatus.Id;

                                    if (orderStatuses.Any())
                                    {
                                        if (order.Histories.Any())
                                        {

                                            List<string> historyStatuses = order.Histories.Select(e => e.StatusId).ToList();
                                            List<OrderStatus> upcomingStatuses = orderStatuses.Where(e => !historyStatuses.Contains(e.Id)).ToList();
                                            if (!upcomingStatuses.Any())
                                            {
                                                order.OrderState = OrderState.Closed;
                                            }
                                        }
                                    }


                                    _context.Orders.Update(order);
                                    saveResult = await _context.SaveChangesAsync();

                                    if (saveResult > 0)
                                    {
                                        string statusMessage = string.IsNullOrEmpty(request.OrderStatusComment) ? null : await SendNotification(
                                        new OrderStatusNotificationRequest
                                        {
                                            Message = request.OrderStatusComment,
                                            Option = history.NotificationOption,
                                            Order = order
                                        });




                                        return new OrderStatusHistoryResponse
                                        {
                                            Histories = await StatusHistories(@request.OrderId),
                                            Response = new BaseResponse
                                            {
                                                Message = string.IsNullOrEmpty(statusMessage) ? "Order status history maintained" : "Order status history maintained. " + statusMessage,
                                                Result = RequestResult.Success,
                                                Succeeded = true,
                                                Title = "Successful"
                                            }
                                        };

                                    }




                                }



                                return new OrderStatusHistoryResponse
                                {
                                    Histories = await StatusHistories(@request.OrderId),
                                    Response = new BaseResponse
                                    {
                                        Message = "Status history couldn't be maintained. Please try again later.",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Unknown Error Occurred"
                                    }
                                };

                             }

                            }

                            else // update/edit mode
                            {

                                if(order.Histories.Any(e => e.StatusId == @request.OrderStatusId))
                                {

                                    if (await _context.OrderStatusHistories.AnyAsync(e => e.Id == @request.OrderHistoryId))
                                    {
                                        OrderStatusHistory orderStatusHistory = order.Histories.Where(e => e.StatusId == @request.OrderStatusId).FirstOrDefault(); 
                                        if (orderStatusHistory != null)
                                        {


                                            orderStatusHistory.ColorCode = orderStatus.ColorCode;
                                            orderStatusHistory.Comment = @request.OrderStatusComment;
                                            orderStatusHistory.NotificationOption = (NotificationOption)Enum.Parse(typeof(NotificationOption), request.OrderStatusNotification);
                                            orderStatusHistory.Icon = orderStatus.Icon;
                                            orderStatusHistory.Label = orderStatus.Label;
                                            orderStatusHistory.OrderId = order.Id;
                                            orderStatusHistory.StatusId = orderStatus.Id;


                                            _context.OrderStatusHistories.Update(orderStatusHistory);
                                            int saveResult = await _context.SaveChangesAsync();




                                            if (saveResult > 0)
                                            {


                                               

                                              
                                                    string statusMessage = string.IsNullOrEmpty(request.OrderStatusComment) ? null : await SendNotification(
                                                 new OrderStatusNotificationRequest
                                                 {
                                                     Message = request.OrderStatusComment,
                                                     Option = orderStatusHistory.NotificationOption,
                                                     Order = order
                                                 });




                                                    return new OrderStatusHistoryResponse
                                                    {
                                                        Histories = await StatusHistories(@request.OrderId),
                                                        Response = new BaseResponse
                                                        {
                                                            Message = string.IsNullOrEmpty(statusMessage) ? "Order status history updated" : "Order status history updated. " + statusMessage,
                                                            Result = RequestResult.Success,
                                                            Succeeded = true,
                                                            Title = "Successful"
                                                        }
                                                    };
                                                






                                            }



                                            return new OrderStatusHistoryResponse
                                            {
                                                Histories = await StatusHistories(@request.OrderId),
                                                Response = new BaseResponse
                                                {
                                                    Message = "Status history couldn't be updated. Please try again later.",
                                                    Result = RequestResult.Error,
                                                    Succeeded = false,
                                                    Title = "Unknown Error Occurred"
                                                }
                                            };


                                        }
                                    }


                                    else
                                    {
                                        return new OrderStatusHistoryResponse
                                        {
                                            Histories = await StatusHistories(@request.OrderId),
                                            Response = new BaseResponse
                                            {
                                                Message = "Order status history not found. This history may have been deleted",
                                                Result = RequestResult.Error,
                                                Succeeded = false,
                                                Title = "Null History Reference"
                                            }
                                        };

                                    }

                                }



                                else
                                {
                                    return new OrderStatusHistoryResponse
                                    {
                                        Histories = await StatusHistories(@request.OrderId),
                                        Response = new BaseResponse
                                        {
                                            Message = "Status: '"+orderStatus.Label+"' not maintained for order yet. Request terminated.",
                                            Result = RequestResult.Error,
                                            Succeeded = false,
                                            Title = "Mismatched Request"
                                        }
                                    };

                                }



                               

                            }


                           

                        }

                        return new OrderStatusHistoryResponse
                        {
                            Histories = await StatusHistories(@request.OrderId),
                            Response = new BaseResponse
                            {
                                Message = "Order status not found. This order status may have been deleted",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Null Status Reference"
                            }
                        };

                    }



                   


                }


                return new OrderStatusHistoryResponse
                {
                    Histories = new List<OrderModelHistory>(),
                    Response = new BaseResponse
                    {
                        Message = "Order not found. This order may have been deleted",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null Order Reference"
                    }
                };

            }
            catch (Exception ex)
            {

                return new OrderStatusHistoryResponse
                {
                    Histories = await StatusHistories(@request.OrderId),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
                
            }
        }

        public async Task<OrderStatusCRUDResponse> Set(OrderStatusRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (@request.Statuses.Any())
                {

                    List<OrderStatus> newStatuses = @request.Statuses.Where(e => string.IsNullOrEmpty(e.Id)).Select(s => new OrderStatus
                    {
                        Id = s.Id,
                        ColorCode = s.ColorCode,
                        Icon = s.Icon,
                        IsDefault = s.IsDefault,
                        Label = s.Label,

                    }).ToList();

                    if (@request.Statuses.Any(e => !string.IsNullOrEmpty(e.Id)))
                    {

                       
                        List<string> Ids = @request.Statuses.Where(e => !string.IsNullOrEmpty(e.Id)).Select(e => e.Id).ToList();
                        List<OrderStatus> _oldStatuses = await _context.OrderStatuses.ToListAsync();
                        List<OrderStatus> updatedStatuses = _oldStatuses.Where(s => Ids.Contains(s.Id)).ToList();
                        List<OrderStatus> _deletedStatuses = _oldStatuses.Where(e => !Ids.Contains(e.Id)).ToList();

                        if (_deletedStatuses.Any())
                        {
                            List<string> deletedIds = _deletedStatuses.Select(e => e.Id).ToList();
                            if (await _context.OrderStatusHistories.AnyAsync(e => deletedIds.Contains(e.StatusId)))
                            {
                                List<OrderStatusHistory> orderHistories = await _context.OrderStatusHistories.Where(e => deletedIds.Contains(e.StatusId)).ToListAsync();
                                if (orderHistories.Any())
                                {
                                    foreach(OrderStatusHistory orderHistory in orderHistories)
                                    {
                                        orderHistory.StatusId = null;
                                    }


                                    _context.OrderStatusHistories.UpdateRange(orderHistories);
                                    await _context.SaveChangesAsync();
                                }
                            }


                            if (await _context.Orders.AnyAsync(e => deletedIds.Contains(e.StatusId)))
                            {
                                List<Order> orders = await _context.Orders.Where(e => deletedIds.Contains(e.StatusId)).ToListAsync();
                                if (orders.Any())
                                {
                                    foreach (Order order in orders)
                                    {
                                        order.StatusId = null;
                                    }


                                    _context.Orders.UpdateRange(orders);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            _context.OrderStatuses.RemoveRange(_deletedStatuses);
                            await _context.SaveChangesAsync();

                        }

                        if (_oldStatuses.Any())
                        {

                            List<string> statses = _oldStatuses.Select(e => e.Id).ToList();
                            List<OrderStatusHistory> orderHistories = await _context.OrderStatusHistories.Where(e => statses.Contains(e.StatusId)).ToListAsync();
                            foreach(OrderStatus orderStatus in updatedStatuses)
                            {
                                OrderStatus requestStatus = @request.Statuses.Where(e => e.Id == orderStatus.Id).Select(s => new OrderStatus
                                {
                                    Id = s.Id,
                                    ColorCode = s.ColorCode,
                                    IsDefault = s.IsDefault,
                                    Icon = s.Icon,
                                    Label = s.Label
                                }).FirstOrDefault();

                                if(requestStatus != null)
                                {
                                    orderStatus.ColorCode = requestStatus.ColorCode;
                                    orderStatus.Icon = requestStatus.Icon;
                                    orderStatus.IsDefault = requestStatus.IsDefault;
                                    orderStatus.Label = requestStatus.Label;

                                    if (orderHistories.Any())
                                    {
                                        List<OrderStatusHistory> _orderHistories = orderHistories.Where(e => e.StatusId == orderStatus.Id).ToList();
                                        if(_orderHistories.Any())
                                        {
                                            foreach(OrderStatusHistory _orderHistory in _orderHistories)
                                            {
                                                _orderHistory.ColorCode = requestStatus.ColorCode;
                                                _orderHistory.Icon = requestStatus.Icon;
                                                _orderHistory.Label = requestStatus.Label;
                                            }
                                        
                                        }
                                    }

                                }
                            }


                            if (orderHistories.Any())
                            {
                                _context.OrderStatusHistories.UpdateRange(orderHistories);
                                await _context.SaveChangesAsync();
                            }
                            _context.OrderStatuses.UpdateRange(_oldStatuses);
                            int updateResult =  await _context.SaveChangesAsync();

                            if(updateResult > 0 && (!newStatuses.Any())) // update result and return since there are no new items
                            {
                                return new OrderStatusCRUDResponse
                                {
                                    Statuses = await Statuses(),
                                    Response = new BaseResponse
                                    {
                                        Message = "Order statuses updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }


                        }

                       

                    }


                  
                       


                        if (newStatuses.Any())
                        {
                            _context.OrderStatuses.AddRange(newStatuses);
                            int addResult = await _context.SaveChangesAsync();
                            if (addResult > 0)
                            {

                                return new OrderStatusCRUDResponse
                                {
                                    Statuses = await Statuses(),
                                    Response = new BaseResponse
                                    {
                                        Message = @request.Statuses.Any(e => !string.IsNullOrEmpty(e.Id)) ? "Order statuses updated and new statues added." : "Order statuses added.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };
                            }
                        }





                    return new OrderStatusCRUDResponse
                    {
                        Statuses = await Statuses(),
                        Response = new BaseResponse
                        {
                            Message = "An unkown error occurred. Request terminated.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Unknown Error"
                        }
                    };





                }

                return new OrderStatusCRUDResponse
                {
                    Statuses = await Statuses(),
                    Response = new BaseResponse
                    {
                        Message = "No order statuses sent. Request termnated.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Empty Request"
                    }
                };

                
            }
            catch (Exception ex)
            {

                return new OrderStatusCRUDResponse
                {
                    Statuses = await Statuses(),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }



        public async Task<string> SendNotification(OrderStatusNotificationRequest request)
        {
            try
            {




                switch (request.Option)
                {
                    case NotificationOption.EmailAndMessage:

                        if (request.Order.Customer != null)
                        {

                        
                        MessageSendStatus messageSent = await _messageService.SendNotificationMessage(new NotificationChat
                        {
                            Attachment = request.Order.Items.Any() ? new ChatBodyAttachment
                            {
                                AttachmentAbsPath = request.Order.Items.First().ImageAbsURL,
                                AttachmentFileType = FileType.Image,
                                AttachmentPath = request.Order.Items.First().ImageURL,
                                AttachmentRelPath = request.Order.Items.First().ImageRelURL,
                                AttachmentTitle = "Order ID: " + request.Order.InvoiceNumber


                            } : null,
                            Message = request.Message,
                            UserId = request.Order.Customer.UserId,
                        });

                        EmailSendStatus mailSent = await _sMTPService.SendOrderStatusNotification(new OrderStatusMail
                        {
                            EmailAddress = request.Order.EmailAddress,
                            Message = request.Message,
                            OrderId = request.Order.InvoiceNumber
                        });

                        if (messageSent.Succeeded && mailSent.Succeeded)
                        {
                            return "Mail and message notifications sent";
                        }


                        else if (messageSent.Succeeded && (!mailSent.Succeeded))
                        {
                            return "Message notification sent but mail notification couldn't be dispatched due to an unknown error";
                        }

                        else if ((!messageSent.Succeeded) && mailSent.Succeeded)
                        {
                            return "Email notification sent but message notification couldn't be dispatched due to an unknown error";
                        }


                        return "An error occurred. Both email and message notification couldn't be sent.";

                      }
                        return string.Empty;



                    case NotificationOption.EmailOnly:
                        EmailSendStatus EmailSent = await _sMTPService.SendOrderStatusNotification(new OrderStatusMail
                        {
                            EmailAddress = request.Order.EmailAddress,
                            Message = @request.Message,
                            OrderId = request.Order.Id
                        });

                        if (EmailSent.Succeeded)
                        {
                            return "Email notification sent";
                        }

                        return "An error occurred. Email notification couldn't be sent";
                       
                        

                    case NotificationOption.MessageOnly:

                        if(request.Order.Customer != null)
                        {
                            MessageSendStatus messageSent = await _messageService.SendNotificationMessage(new NotificationChat
                            {
                                Attachment = request.Order.Items.Any() ? new ChatBodyAttachment
                                {
                                    AttachmentAbsPath = request.Order.Items.First().ImageAbsURL,
                                    AttachmentFileType = FileType.Image,
                                    AttachmentPath = request.Order.Items.First().ImageURL,
                                    AttachmentRelPath = request.Order.Items.First().ImageRelURL,
                                    AttachmentTitle = "Order ID: " + request.Order.InvoiceNumber


                                } : null,
                                Message = request.Message,
                                UserId = request.Order.Customer.UserId
                            });


                            if (messageSent.Succeeded)
                            {
                                return "Message notification sent";
                            }

                            return "An error occurred. Message notification could not sent.";
                        }

                        return string.Empty;



                    default:

                        return string.Empty;
                        



                }

              
                
            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<List<OrderModelHistory>> StatusHistories(string orderId)
        {
            try
            {
                if(await _context.OrderStatusHistories.AnyAsync(e => e.OrderId == orderId))
                {


                    return await _context.OrderStatusHistories.Where(e => e.OrderId == orderId).OrderBy(e => e.CreatedOn).Select(h => new OrderModelHistory
                    {
                        ColorCode = h.Status.ColorCode,
                        Date = h.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt),
                        Icon = h.Status.Icon,
                        NotificationOption = h.NotificationOption.ToString(),
                        OrderId = h.OrderId,
                        Comment = h.Comment,
                        StatusId = h.StatusId,
                        Id = h.Id,
                        Label = h.Status.Label

                    }).ToListAsync();

                }


                return new List<OrderModelHistory>();
            }
            catch (Exception)
            {

                return new List<OrderModelHistory>();
            }
        }


        private async Task<List<OrderModelStatusBase>> Statuses()
        {
            try
            {
                return await _context.OrderStatuses.OrderByDescending(e => e.IsDefault).ThenBy(e => e.CreatedOn).Select(e => new OrderModelStatusBase
                {
                    Id = e.Id,
                    Label = e.Label,
                    IsDefault = e.IsDefault,
                    ColorCode = e.ColorCode,
                    Icon = e.Icon
                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DefNotificationResponse> DefNotification(DefNotificationModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (string.IsNullOrEmpty(request.Id))
                {
                    DefNotification defNotification = new()
                    {
                        Message = @request.Message,
                        NotificationOption = (NotificationOption)Enum.Parse(typeof(NotificationOption), request.Option)

                    };


                    _context.DefNotifications.Add(defNotification);
                    int saveRes = await _context.SaveChangesAsync();

                    if(saveRes > 0)
                    {

                        return new DefNotificationResponse
                        {
                            DefNotification = new DefNotificationModel
                            {
                                Id = defNotification.Id,
                                Message = defNotification.Message,
                                Option = defNotification.NotificationOption.ToString()
                            },

                            Response = new BaseResponse
                            {
                                Message = "Default notification configured.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                      
                    }
                }


                else
                {
                    if(await _context.DefNotifications.AnyAsync(e => e.Id == @request.Id))
                    {
                        DefNotification defNotification = await _context.DefNotifications.FindAsync(request.Id);
                        if(defNotification != null)
                        {
                            defNotification.Message = @request.Message;
                            defNotification.NotificationOption = (NotificationOption)Enum.Parse(typeof(NotificationOption), request.Option);

                            _context.DefNotifications.Update(defNotification);
                            int upResult =  await _context.SaveChangesAsync();

                            if(upResult > 0)
                            {

                                return new DefNotificationResponse
                                {
                                    DefNotification = new DefNotificationModel
                                    {
                                        Id = defNotification.Id,
                                        Message = defNotification.Message,
                                        Option = defNotification.NotificationOption.ToString()
                                    },

                                    Response = new BaseResponse
                                    {
                                        Message = "Default notification configuration updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    }
                                };



                              
                            }
                        }
                    }


                    return new DefNotificationResponse
                    {
                        DefNotification = new DefNotificationModel
                        {
                            Id = @request.Id,
                            Message = @request.Message,
                            Option = @request.Option
                        },

                        Response = new BaseResponse
                        {
                            Message = "Null defualt notification config reference. This configuration may have been deleted.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Null Configuration Reference"
                        }
                    };


                    
                }



                return new DefNotificationResponse
                {
                    DefNotification = new DefNotificationModel
                    {
                        Id = @request.Id,
                        Message = @request.Message,
                        Option = @request.Option
                    },

                    Response = new BaseResponse
                    {
                        Message = "Default notification could not be maintained. Please try again later.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Unknown Error"
                    }
                };


             
            }
            catch (Exception ex)
            {



                return new DefNotificationResponse
                {
                    DefNotification = new DefNotificationModel
                    {
                        Id = @request.Id,
                        Message = @request.Message,
                        Option = @request.Option
                    },

                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };



            
            }
        }
    }
}
