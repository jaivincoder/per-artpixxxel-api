

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Orders;
using api.artpixxel.data.Models;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.OrderStatuses
{
    public class OrderModelStatusBase
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool IsDefault { get; set; }
        public string ColorCode { get; set; }
        public string Icon { get; set; }
    }





    public class OrderModelStatus : OrderModelStatusBase
    {
        public string Comment { get; set; }
    }

    public class OrderStatusRequest
    {
        public List<OrderModelStatusBase> Statuses { get; set; }
    }




    public class OrderStatusCRUDResponse : OrderStatusRequest
    {
        public BaseResponse Response { get; set; }
    }



    public class OrderStatusMail
    {
        public string EmailAddress { get; set; }
        public string Message { get; set; }
        public string OrderId { get; set; }
    }


    public class OrderStatusHistoryRequest
    {
        public string OrderId { get; set; }
        public string OrderHistoryId { get; set; }
        public string OrderStatusNotification { get; set; }
        public string OrderStatusId { get; set; }
        public string OrderStatusComment { get; set; }
    }


    public class OrderStatusHistoryResponse
    {
        public List<OrderModelHistory> Histories { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class DefNotificationResponse
    {
        public DefNotificationModel DefNotification { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class DefNotificationModel
    {
        public string Id { get; set; }
        public string Option { get; set; }
        public string Message { get; set; }
    }

    public class OrderStatusNotificationRequest
    {
        public NotificationOption Option { get; set; }
        public Order Order { get; set; }
        public string Message { get; set; }
    }

   
}


