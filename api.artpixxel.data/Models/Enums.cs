

namespace api.artpixxel.data.Models
{
    public enum Gender
    {
        Male = 0,
        Female = 1
    }

    public enum ReadStatus
    {
        Unread = 0,
        Read = 1
    }


    public enum AccessType
    {
        General = 0,
        Specific = 1
    }


    public enum NotificationPriority
    {
        Urgent = 0,
        Default = 1,
        Negligible = 2
    }

   public enum PaymentStatus{
        Paid = 0,
        Unpaid = 1
    }
    public enum OrderItemCategory
    {
        UploadedImage = 0,
        MixnMatch = 1,
        WallArt = 2,
        
    }

    public enum OrderType
    {
        Regular = 0,
        MixedTemplate = 1,
        KidsTemplate = 2,
        UploadRegular = 3,
        FloralRegular = 4,
        KidsRegular = 5,
        UploadTemplate = 6,
        ChristmasRegular = 7,
        ChristmasTemplate = 8

    }

    public enum OrderState
    {
        Open = 0,
        Closed = 1
    }

    public enum MetaType
    {
        UploadedImage = 0,
        MixnMatch = 1,
        VAT = 2,
        PublishableKey = 3,
        SecretKey = 4,
        KidsGalleryImage = 5
    }


    public enum TimeLimit
    {
        Hour = 0,
        Day = 1,
        Week = 2,
        Month = 3,
        Year = 4
    }


    public enum NotificationOption
    {
        EmailAndMessage = 0,
        EmailOnly = 1,
        MessageOnly = 2,
        DoNotNotify = 3
    }

    public enum FileType
    {
        Unkown = 0,
        Image = 1,
        Audio= 2,
        Video = 3,
        PDF = 4,
        Doc = 5,
        XLS = 6,
        EPS = 7,
        SVG = 8,
        GIF = 9,
        TXT = 10,
        PPT = 11,
      
    }


    public enum SizeType
    {
        Regular = 0,
        FloralLiner = 1,
        KidsSpace = 2,
        Christmas = 3
    }
}


