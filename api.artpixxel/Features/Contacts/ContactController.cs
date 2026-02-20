using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Contacts;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.Contacts
{

    public class ContactController : ApiController
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService= contactService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<BaseResponse> Create(ContactRequest request)
            => await _contactService.Create(request);


        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Multiple))]
        public async Task<BaseBoolResponse> Multiple(MultipleContactRequest request)
            => await _contactService.Multiple(request);

        [HttpPost]
        [Route(nameof(List))]
        public async Task<ContactListInit> List(Pagination pagination)
            => await _contactService.List(pagination);
    }
}
