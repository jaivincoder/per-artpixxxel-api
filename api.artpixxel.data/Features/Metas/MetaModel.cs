

using api.artpixxel.data.Features.Common;

namespace api.artpixxel.data.Features.Metas
{

    public class UploadImagePrice
    {
        public decimal UploadedImagePrice { get; set; }
      
    }
    public class MetaBase  
    {
       public string UploadedImagePrice { get; set; }
       public string KidsGalleryImagePrice { get; set; }
       public string MixNMatchPrice { get; set; }
       public string VATAmount { get; set; }
       public string PublishableKey { get; set; }
       public string SecretKey { get; set; }

    }
    public class MetaModel
    {
      public string Type { get; set; }
      public string Value { get; set; }
      public string DateSet { get; set; }
    }

    public class MetaModelBase
    {
       public MetaModel UploadedImage { get; set; }
        public MetaModel KidsGalleryImage { get; set; }
        public MetaModel MixNMatch { get; set; }
       public MetaModel VAT { get; set; }
       public MetaModel PublishableKey { get; set; } 
       public MetaModel SecretKey { get; set;}
    }


    public class MetaResponse
    {
        public MetaModelBase Meta { get; set; }
        public BaseResponse Response { get; set; }
    }






}
