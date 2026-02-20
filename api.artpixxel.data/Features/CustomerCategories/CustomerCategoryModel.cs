

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.CustomerCategories
{
    public class CustomerCategoryRequest
    {
        public string CustomerCategoryId { get; set; }
        public string CustomerCategoryName { get; set; }
        public string CustomerCategoryColorCode { get; set; }
        public string CustomerCategoryDescription { get; set; }
        public bool CustomerCategoryDefault { get; set; }


    }

    public class CustomerCategoryResponse : CustomerCategoryRequest
    {
        public decimal CustomerCategoryCustomerCount { get; set; }
    }

    public class CustomerCategoryCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public List<CustomerCategoryResponse> Categories { get; set; }
    }

    public class CustomerCategoryBatchDelete
    {
        public List<string> Ids { get; set; }
    }
}
