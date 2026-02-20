using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.TemplateConfing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Features.GalleryImages
{
    public class GalleryImagesModel
    {
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
        public List<ImagesDataModel> galleryImage { get; set; }
    }
    public class ImagesDataModel
    {
        public string ImagePath { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class GalleryModalAddData
    {
        public int CategoryId { get; set; }
        public List<GalleryImagesItemDto> galleryImage { get; set; }
    }
    public class GalleryImagesCRUDResponse : BaseResponse
    {
        public GalleryModalAddData Data { get; set; }
    }
    public class GalleryImageUpdateRequest
    {
        public IFormFile ImagePath { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }

    public class GalleryImagesGroupDto 
    {
        public int CategoryId { get; set; }
        public List<GalleryImagesItemDto> galleryImage { get; set; }
    }

    public class GetGalleryImageResponse : BaseResponse
    {
        public GalleryImagesGroupDto Data { get; set; }
    }
    public class GetGalleryImageListResponse : BaseResponse
    {
        public List<GalleryImagesGroupDto> Data { get; set; }
    }
    public class GalleryImageCRUDResponse : BaseResponse
    {
        public List<GalleryImagesModel> Data { get; set; }
    }
    public class GalleryImagesItemDto
    {
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public int OrderNo { get; set; }
        public bool IsActive { get; set; }

    }
}
