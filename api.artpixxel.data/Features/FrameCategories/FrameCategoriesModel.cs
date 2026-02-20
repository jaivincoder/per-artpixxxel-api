using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Features.FrameCategories
{
    
    public class FrameCategoryModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string GalleryTitle { get; set; }

        public string CategoryType { get; set; }
        public List<LineColor> LineColors { get; set; }
        public List<GetGalleryImage> GalleryImage { get; set; }
    }

    public class FrameCategoryCRUDResponse :BaseResponse
    {
        public FrameCategoryModel Data { get; set; }
    }
   
   
   

}
