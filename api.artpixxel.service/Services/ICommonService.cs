using api.artpixxel.data.Features.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public  interface ICommonService
    {
        Task<List<CountryOption>> Countries();
        Task<List<BaseOption>> States(BaseId request);
        Task<List<BaseOption>> Cities(BaseId request);
        Task<LocationModel> LocationInfo();
        Task<ShoppingInfo> ShoppingInfo();
    }
}
