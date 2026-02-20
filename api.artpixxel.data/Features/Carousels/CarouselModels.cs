

using api.artpixxel.data.Features.Common;
using System.Collections.Generic;

namespace api.artpixxel.data.Features.Carousels
{
   public class CarouselModel
    {
        public string Id { get; set; }
        public string Heading { get; set; }
        public string HeadingColour { get; set; }
        public string BodyText { get; set; }
        public string BodyTextColour { get; set; }
        public string BackgroundColour { get; set; }
        public string Link { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public string LinkLabel { get; set; }
        public string LinkLabelColour { get; set; }
        public string Image { get; set; }
    }

    public class CarouselResponse
    {
        public List<CarouselModel> Carousels { get; set; }
        public decimal TotalCarousels { get; set; }
    }

    public class CarouselCRUDResponse 
    {
        public CarouselResponse Carousel { get; set; }
        public BaseResponse Response { get; set; }
    }

    public class CarouselRequest
    {
        public CarouselModel Request { get; set; }
        public Filter Filter { get; set; }
    }

    public class BatchCarouselRequest
    {
        public List<CarouselModel> Request { get; set; }
        public Filter Filter { get; set; }
    }


    public class CarouselDeleteRequest
    {
        public string Id { get; set; }
        public Filter Filter { get; set; }

    }

}
