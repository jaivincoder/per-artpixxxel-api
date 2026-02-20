using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Emails
{
 
    public class EmailController : ApiController
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
           _emailService = emailService;
        }


        [HttpPost]
        [Route(nameof(Init))]
        public async Task<EmailListInit> Init(Pagination pagination)
            => await _emailService.Init(pagination);


        [HttpPost]
        [Route(nameof(MailList))]
        public async Task<EmailListInit> MailList(EmailListFilterData filter)
            => await _emailService.MailList(filter);


        [HttpPost]
        [Route(nameof(MultiDelete))]
        public async Task<EmailListCRUDResponse> MultiDelete(EmailListMultiDeleteRequest request)
            => await _emailService.MultiDelete(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<EmailListCRUDResponse> Delete(EmailListDeleteRequest request)
            => await _emailService.Delete(request);

        [HttpPost]
        [Route(nameof(Export))]
        public async Task<List<EmailListResponse>> Export(EmailListFilterData filter)
            => await _emailService.Export(filter);


    }
}
