using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.data.Models
{
    public class Template_Configs : AuditedDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TemplateKey { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public frame_categories Category { get; set; }

        [Required]
        public string PreviewImage {  get; set; }

        [Required]
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

    }


    public class TemplateFrameMappingRequestDto
    {
        public int TemplateConfigId { get; set; }
        public int OrderNo { get; set; }
        public string FrameId { get; set; }
        public string SvgPath { get; set; }
        public string Thumbnail { get; set; }
        public bool IsActive { get; set; }
    }
    public class TemplateWithCategoryDto
    {
        public int Id { get; set; }
        public string TemplateKey { get; set; }
        public int CategoryId { get; set; }
        public string PreviewImage { get; set; }
        public decimal Price { get; set; }
        public string CategoryType { get; set; }
        public string CategoryLabel { get; set; }
        public bool IsActive { get; set; }
        public List<TemplateFrameMappingRequestDto> TemplateFramesMapping { get; set; }
    }
}
 