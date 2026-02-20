using Microsoft.AspNetCore.Http;
using System;

namespace api.artpixxel.data.Features.Frames
{
    public class FrameRequest
    {
        public string frame_type { get; set; }
        public string frame_image { get; set; }
        public IFormFile file { get; set; }
        public DateTime? date { get; set; }
        public string createdBy { get; set; }

     
    }
    public class FrameRequestNew
    {
        public string FrameType { get; set; } 
        public string FrameSet { get; set; } 

        public decimal FramePrice { get; set; }
        public string Name { get; set; }
        public IFormFile SvgPath { get; set; }
        public bool? IsCharacter { get; set; }
        public bool IsActive { get; set; }
        public int? DisplayOrder { get; set; }
        public IFormFile Thumbnail { get; set; }
        public int CategoryId { get; set; }
    }

    public class FrameUpdateRequest
    {
        public string Type { get; set; }
        public IFormFile SvgPath { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class FrameDeleteRequest
    {
        public string Id { get; set; }
    }
   
}