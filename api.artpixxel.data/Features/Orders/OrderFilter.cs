using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Orders
{
  public  class OrderFilter
    {
      public List<string> Cities { get; set; }
      public List<string> Countries { get; set; }
      public string OrderDate { get; set; }
      public string OrderDateMatchMode { get; set; }
      public List<string> States { get; set; }
      public List<string> FullNames { get; set; }
      public List<string> OrderStates { get; set; }
      public List<string>  OrderStatuses { get; set; }
      public decimal SubTotal { get; set; }
      public string SubTotalMatchMode { get; set; }
    }

    public class OrderFilterData : Filter
    {
        public OrderFilter Filters { get; set; }

    }

    public class OrderSortField
    {
        public const string INVOICENUMBER = "invoiceNumber";
        public const string FULLNAME = "fullName";
        public const string COUNTRY = "country";
        public const string STATE = "state";
        public const string CITY = "city";
        public const string STATUS = "status";
        public const string ORDERSTATE = "orderState";
        public const string DATEFILTERED = "dateFiltered";
        public const string SUBTOTAL = "subTotal";
    }
}
