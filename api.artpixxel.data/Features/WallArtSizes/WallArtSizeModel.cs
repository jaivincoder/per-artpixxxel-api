

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.WallArtSizes
{

    public class WallArtSizeBase
    {
        public string WallArtSizeId { get; set; }
        public string WallArtSizeName { get; set; }
        public decimal WallArtSizeAmount { get; set; }
        public bool IsWallArtSizeDefault { get; set; }
        public string WallArtSizeDescription { get; set; }
    }
   public class WallArtSizeResponse : WallArtSizeBase
    {
        
        public decimal WallArtSizeWallArtCount { get; set; }
      
    }



    public class WallArtSizeOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public decimal Amount { get; set; }
        public decimal WallArtCount { get; set; }
    }

    public class WallArtSizeCRUDResponse
    {
        public List<WallArtSizeResponse> WallArtSizes { get; set; }
        public BaseResponse Response { get; set; }
    }

    public class PublicWallArtSize
    {
      


        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
