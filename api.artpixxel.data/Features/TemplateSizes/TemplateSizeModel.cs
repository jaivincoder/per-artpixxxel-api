

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.TemplatesSizes
{
    public class TemplateSizeModel
    {
       public string SizeId { get; set; }
       public string SizeName { get; set; }
       public float SizeWidth { get; set; }
       public float SizeHeight { get; set; }
       public decimal SizeAmount { get; set; }
       public bool SizeDefault { get; set; }
       public string SizeTemplate { get; set; }
       public string SizeDescription { get; set; }
    }

    public class TemplateSizCRUDResponse
    {
        public List<TemplateSizeModel> Sizes { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class DuplicateTemplateSize : BaseOption
    {
        public string Template { get; set; }
    }


    public class TemplateStatistics
    {
        public int SizeCount { get; set; }
        public string Template { get; set; }
    }

    public class TemplateStatisticsResponse
    {
        public List<TemplateStatistics> Statistics { get; set; }
        public BaseResponse Response { get; set; }
    }


   
}
