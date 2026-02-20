

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.MixnMatches;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.MixnMatchCategories
{
   public class MixNMatchCategoryCRUDResponse
    {
        public BaseResponse Response { get; set; }
        public List<BaseOption> Categories { get; set; }
    }

    public class MixnMatchCategoryRequest
    {
       
        public string MixnmatchCategoryId { get; set; }
        public string MixnmatchCategoryName { get; set; }
        public string MixnmatchCategoryDescription { get; set; }
    }

    public class MixnMatchCategoryBase
    {
        public string MixnmatchCategoryId { get; set; }
        public string MixnmatchCategoryName { get; set; }
        public string MixnmatchCategoryDescription { get; set; }
        public decimal MixnmatchCategoryImageCount { get; set; }
    }

    public class MixnMatchCategoryCRUD
    {
        public BaseResponse Response { get; set; }
        public List<MixnMatchCategoryBase> Categories { get; set; }
    }


    public class CategoryMixnMatch
    {
        public MixnMatchResponse MixnMatches { get; set; }
        
    }

}
