using api.artpixxel.data.Features.AddressBooks;
using api.artpixxel.service.Services;
using Artpixxel.server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.artpixxel.Features.AddressBooks
{
    public class AddressBookController : ApiController
    {
        private readonly IAddressBookService _addressBookService;
        public AddressBookController(IAddressBookService addressBookService)
        {
            _addressBookService = addressBookService;
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<AddressBookCRUDResponse> Create(AddressBookRequest request)
            => await _addressBookService.Create(request);

        [HttpPost]
        [Route(nameof(Delete))]
        public async Task<AddressBookCRUDResponse> Delete(AddressBookDelete request)
            => await _addressBookService.Delete(request);
    }
}
