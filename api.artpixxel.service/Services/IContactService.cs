

using api.artpixxel.data.Features.Cities;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Contacts;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public interface IContactService
    {
        Task<BaseResponse> Create(ContactRequest request);
        Task<BaseBoolResponse> Multiple(MultipleContactRequest request);
        Task<ContactListInit> List(Pagination pagination);
    }
}
