

using api.artpixxel.data.Features.AddressBooks;
using api.artpixxel.data.Features.Common;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IAddressBookService
    {
        Task<AddressBookCRUDResponse> Create(AddressBookRequest request);
        Task<AddressBookCRUDResponse> Delete(AddressBookDelete request);
    }
}
