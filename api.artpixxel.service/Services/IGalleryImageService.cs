using api.artpixxel.data.Features.GalleryImages;
using api.artpixxel.data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.service.Services
{
    public  interface IGalleryImageService
    {
        Task<GetGalleryImageListResponse> GetGalleryImages();
        Task<GetGalleryImageResponse> GetGalleryImageById(int id);
        Task<GalleryImagesCRUDResponse> AddGalleryImage(GalleryImagesModel request);
        Task<GetGalleryImageResponse> UpdateGalleryImage(int id,int CategoryId, GalleryImageUpdateRequest request);
        Task<GalleryImageCRUDResponse> DeleteGalleryImage(int id);
    }
}
