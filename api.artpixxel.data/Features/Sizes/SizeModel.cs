

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Sizes
{
   public class SizeModel
    {
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        public string SizeType { get; set; }
        public double SizeWidth { get; set; }
        public double SizeHeight { get; set; }
        public decimal SizeAmount { get; set; }
        public bool SizeDefault { get; set; }
        public string SizeDescription { get; set; }
    }



    public class DuplicateSize: BaseOption
    {
        public string SizeType { get; set; } 
    }


    public class SizeCRUDResponse
    {
        public List<SizeModel> Sizes { get; set; }
        public BaseResponse Response { get; set; }
    }

    public class PublicSize
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string AspectRatio { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool Default { get; set; }
    }

    public class PublicTemplateSize
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string TemplateName { get; set; }
        public string TemplateType { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Description { get; set; }
        public bool Default { get; set; }
    }



}
