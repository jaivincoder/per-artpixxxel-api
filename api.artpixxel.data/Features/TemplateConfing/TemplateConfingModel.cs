using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Features.TemplateConfing
{
    public class TemplateConfingModel
    {

        public string TemplateKey { get; set; }

        public int CategoryId { get; set; }

        public string PreviewImage { get; set; }

        public decimal Price { get; set; }
         public bool IsActive { get; set; }
        public List<TemplateFramesData> TemplateFramesMapping {  get; set; }

        //public List<int> OrderNo { get; set; }

        //public List<string> FrameId { get; set; }
    }

    public class TemplateFramesData
    {
        public int OrderNo { get; set; }
        public string FrameId { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetTemplateConfingListResponse :BaseResponse
    {
        public List<TemplateWithCategoryDto> Data { get; set; }
    }

    public class GetTemplateConfingResponse : BaseResponse
    {
        public TemplateWithCategoryDto Data { get; set; }
    }

    public class TemplateConfligResponse 
    {
        public int Id { get; set; }
        public string TemplateKey { get; set; }

        public int CategoryId { get; set; }

        public string PreviewImage { get; set; }

        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public List<TemplateFrameMappingRequestDto> TemplateFramesMapping { get; set; }
    }

    public class TemplateConfligCRUDResponse : BaseResponse
    {
        public TemplateConfligResponse Data { get; set; }
    }
}
