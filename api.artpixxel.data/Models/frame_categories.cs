using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace api.artpixxel.data.Models
{
    public class frame_categories : AuditedDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Label { get; set; }

        [MaxLength(100)]
        public string GallaryTitle { get; set; }

        [Required]
        public string CategoryType { get; set; }
        public bool IsActive { get; set; }

    }
    public class FrameCategoryResponseDto
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string GallaryTitle { get; set; }
        public string CategoryType { get; set; }
        public bool IsActive { get; set; }  
        public List<LineColor> LineColors { get; set; }

    }

    public class LineColor
    {
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class GetFrameCategoryListResponse :BaseResponse
    {
        public List<GetFrameCategory> Data { get; set; }
    }

    public class GetFrameCategoryResponse: BaseResponse
    {
        public GetFrameCategory Data { get; set; }
    }

    public class GetFrameCategory 
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string GallaryTitle { get; set; }

        public bool IsActive { get; set; }
        public string CategoryType { get; set; }

        public List<LineColor> LineColors { get; set; }

        public List <GetGalleryImage> GalleryImage { get; set; }
    }

    public class GetGalleryImage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
