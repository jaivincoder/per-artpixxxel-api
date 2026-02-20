using api.artpixxel.data.Features.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Features.Payments
{
  

    public class PublishableKeyResponse
    {
        public string PubKey { get; set; }
    }

    public class PaymentChargeRequest
    {
        public long Amount { get; set; }
        public string EmailAddress { get; set; }
        public string TokenId { get; set; }
        public string InvoiceId { get; set; }
    }


    public class PaymentChargeResponse
    {
        public string ChargeId { get; set; }
        public BaseResponse Response { get; set; }
    }


}
