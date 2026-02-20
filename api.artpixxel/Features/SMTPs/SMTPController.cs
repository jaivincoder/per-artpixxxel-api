using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.SMTPs;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.SMTPs
{
   
    public class SMTPController : ApiController
    {
        private readonly ISMTPService _sMTPService;
        public SMTPController(ISMTPService sMTPService)
        {
            _sMTPService = sMTPService;
        }

        [HttpPost]
        [Route(nameof(Exists))]
        public async Task<BaseBoolResponse> Exists(SMTPModel request)
            => await _sMTPService.Exists(request);

        [HttpPost]
        [Route(nameof(Duplicate))]
        public async Task<BaseBoolResponse> Duplicate(SMTPModel request)
           => await _sMTPService.Duplicate(request);

        [HttpPost]
        [Route(nameof(Init))]
        public async Task<SMTPInit> Init(Pagination pagination)
            => await _sMTPService.Init(pagination);

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<SMTPCRUDResponse> Create(SMTPWriteRequest request)
            => await _sMTPService.Create(request);

        [HttpPost]
        [Route(nameof(Update))]
        public async Task<SMTPCRUDResponse> Update(SMTPWriteRequest request)
            => await _sMTPService.Update(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<SMTPCRUDResponse> Delete(SMTPDeleteRequest request)
            => await _sMTPService.Delete(request);

    }
}
