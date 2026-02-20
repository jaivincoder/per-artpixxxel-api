using api.artpixxel.data.Features.Checkouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface ICheckOutNewService
    {
        Task<CheckoutResponse> CheckoutNew(CheckoutNew checkout);
    }
}
