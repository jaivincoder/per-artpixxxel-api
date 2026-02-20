
using api.artpixxel.data.Features.Carousels;
using api.artpixxel.data.Features.Common;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
   public interface ICarouselService
    {
        Task<CarouselCRUDResponse> Create(CarouselRequest request);
        Task<CarouselCRUDResponse> BatchCreate(BatchCarouselRequest request);
        Task<CarouselCRUDResponse> Delete(CarouselDeleteRequest request);
       Task<CarouselCRUDResponse> Update(CarouselRequest request);
        Task<CarouselResponse> Carousels(Pagination pagination);
        Task<CarouselResponse> CarouselsPaginated(Filter filter);

        Task<CarouselResponse> PublicCarousels();
    }
}
