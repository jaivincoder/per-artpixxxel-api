

using api.artpixxel.data.Models;

namespace api.artpixxel.repo
{
    public class Constants
    {
        public const int MaxNotification = 10;
        public const decimal DefaultPrice = 11m;
        public const TimeLimit LeadTimeLimit = TimeLimit.Day;
        public const int LeadTimeLowerBandQuantifier = 3;
        public const int LeadTimeUpperBandQuantifier = 5;
    }


    public class TemplateType
    {
        public const string KidsTemplate = "KidsTemplate";
        public const string MixedTemplate = "MixedTemplate";
        public const string RegularTemplate = "RegularTemplate";
        public const string ChristmasTemplate = "ChristmasTemplate";

    }


    public class  ImageFileType
    {
        public const string Character = "Character";
        public const string LocalFile = "LocalFile";
    }

    public class FrameClass
    {
        public const string ELEGANT ="elegant"; 
        public const string BOLD ="bold"; 
        public const string SLICK ="slick"; 
        public const string VALIANT ="valiant";
        public const string EDGE = "edge";
        public const string MONO = "mono";
        public const string INK = "ink";
    }
   
    public class framecategories
    {
        public const string CategoryTypeLinearArt = "LinearArt";
        public const string CategoryTypeArtMat = "ArtMat";
    }

    public class TemplateConfig
    {
        public const string template_key_type1 = "setof3set1 ,setof3set2 ,setof3set3,setof3set4,setof3set5,setof3set6,setof3set7,setof4set1,setof4set2,setof4set3,setof4set4,setof4set5,setof4set6,setof4set7," +
            "setof4set8,setof4set9,setof4set10,setof4set11,setof5set1,setof5set2,setof6set1,setof6set2,setof6set3,setof6set4,setof6set5,setof6set6,setof6set7," +
            "setof6set8,setof6set9,setof6set10,setof6set11,setof6set12,setof6set13,setof6set14,setof6set15,setof7set1,setof8set1,setof8set2,setof8set3,setof9set1," +
            "setof9set2,setof9set3,setof9set4,setof9set5,setof9set6,setof9set7,setof9set8,setof9set9,setof9set10,setof9set11,setof9set12,setof9set13,setof12set1,setof12set2,setof15set1";

    } 
    public class DefaultRoles
    {
            public const string Admin = "Admin";
            public const string Employee = "Employee";
            public const string SuperAdmin = "SuperAdmin";
            public const string Customer = "Customer";
    }
    public class DefaultRoleDescripion
    {
        public const string AdminDescription = "Admin role";
        public const string SuperAdminDescription = "SuperAdmin role";
        public const string EmployeeDescription = "Employee role";
        public const string CustomerDescription = "Customer role";
    }

    public class DefaultEmails
    {
        public const string AdminEmail = "yemiogunlolu@gmail.com";
        public const string SuperAdminEmail = "ogundaregbenga2017@gmail.com";
    }


    public class AssetDefault
    {
        public const string DefaultImage = "./assets/images/placeholder.svg";
        public const string DefaultImagePNG = "./assets/images/placeholder.png";
        public const string DefaultUserImage = "./assets/images/user-placeholder.svg";
    }
    public class DefaultUserNames
    {
        public const string Admin = "admin";
        public const string SuperAdmin = "superadmin";
    }

    public class DefaultDateFormat
    {
        public const string ddMMyyyy = "dd/MM/yyyy";
        public const string MMyyyy = "MM/yyyy";
        public const string yyyy = "yyyy";
        public const string ddMMyyyyHHmmss = "dd/MM/yyyy HH:mm:ss";
        public const string ddMMyyyyhhmmss = "dd/MM/yyyy hh:mm:ss";
        public const string dmyyyyHHms = "d/m/yyyy HH:m:s";
        public const string ddMMyyyyhhmmsstt = "dd/MM/yyyy hh:mm:ss tt";
    }


    public class CustomPermission
    {
        public const string CustomerType = "CustomerType";
        public const string HomeSlider = "HomeSlider";
        public const string MixMatch = "MixMatch";
        public const string MixMatchCategory = "MixMatchCategory";
        public const string Order = "Order";
        public const string OrderStatus = "OrderStatus";
        public const string Permission = "Permission";
        public const string State = "State";
        public const string Employee = "Employee";
        public const string WallArt = "WallArt";
        public const string WallArtCategory = "WallArtCategory";
        public const string WallArtSize = "WallArtSize";
        public const string Country = "Country";
        public const string UserRole = "UserRole";
        public const string Frame = "Frame";

    }

    public class DefaultPassword
    {
        public const string SuperAdminDefault = "My@D3vPassw0rd$";
        public const string AdminDefault = "MySecr3t$";
        public const string EmployeeDefault = "MySecr3t$";
        
    }

    public class RequestResult
    {
        public const string Success = "success";
        public const string Error = "error";
        public const string Warn = "warn";
        public const string Info = "info";
    }

    public class MatchMode
    {
        public const string DateIS = "dateIs";
        public const string DateIsNT = "dateIsNot";
        public const string DateBefore ="dateBefore";
        public const string DateAfter ="dateAfter";
    }

    public class Operators
    {
       public const string EQUALS = "equals";
       public const string NOT_EQUALS ="notEquals";
       public const string LESS_THAN ="lt";
       public const string LESS_THAN_OR_EQUALTO = "lte";
       public const string GREATER_THAN = "gt";
       public const string GREATER_THAN_OR_EQUAL_TO ="gte";
    }


    public class PaymentOption
    {
        public const string Card = "card";
    }

    public class Currency
    {
        public const string USD = "usd";
    }
   
}
