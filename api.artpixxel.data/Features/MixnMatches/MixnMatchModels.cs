

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.MixnMatches
{

    public class MixnMatchModel
    {
        public List<MixnMatchContent> MixnMatches { get; set; }
    }
    public class MixnMatchContent
    {
      public string Category { get; set; }
      public List<ImageModel> Images { get; set; }
    }
    public class MixnMatchBase
    {
        public string MixnmatchId { get; set; }
        public string MixnmatchName { get; set; }
        public string MixnmatchCategoryCategoryId { get; set; }
        public string MixnmatchImage { get; set; }
        public string MixnmatchDescription { get; set; }
    }


    public class MixnMatchData : MixnMatchBase
    {
       
        public string MixnmatchCategoryCategoryName { get; set; }
        public string MixnmatchImageURL { get; set; }
        public bool Selected { get; set; }
    }

    public class MixnMatchBatchDelete
    {
        public List<string> MixnMatchIds { get; set; }
        public PaginationFilter PaginationFilter { get; set; }
    }

    public class MixnMatchBatchUpdate :  MixnMatchBatchDelete
    {
       public string CategoryId { get; set; }
       public string Description { get; set; }
    }

    public class MixnMatchResponse
    {
        public List<MixnMatchData> MixNMatchData { get; set; }
        public decimal TotalCount { get; set; }
    }


    public class MixMatchCRUDResponse
    {
        public MixnMatchResponse MixMatches { get; set; }
        public BaseResponse Response { get; set; }
    }


    public class MixMatchDeleteRequest
    {   public BaseId Id { get; set; }
        public PaginationFilter PaginationFilter { get; set; }
    }

   


  

    public class MixnMatchRequest
    {
        public MixnMatchBase MixNMatch { get; set; }
        public PaginationFilter PaginationFilter { get; set; }

    }


    public class MultipleMixnMatchRequest
    {
        public List<MixnMatchBase> MixNMatches { get; set; }
        public PaginationFilter PaginationFilter { get; set; }
    }
}

   