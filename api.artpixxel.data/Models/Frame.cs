using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
    //public class Frame : AuditedDeletableEntity
    //{
    //    [Key]
    //    public string Id { get; set; }
        
    //    [Required]
    //    public string Frame_Type { get; set; }
        
    //    [Required]
    //    public string Frame_Image { get; set; }
        
    //    public DateTime? Date { get; set; }
    //}


    public class Frame : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }

        public string FrameType { get; set; } = string.Empty;

        public string FrameSet { get; set; } = string.Empty;

        public decimal FramePrice { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SvgPath { get; set; } = string.Empty;

        public bool? IsCharacter { get; set; }

        public int? DisplayOrder { get; set; }

        [Required]
        public string Thumbnail { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public frame_categories Category { get; set; }
        public int FrameId { get; set; }

        public bool IsActive { get; set; }
    }



   public class FrameDto
    {
        public string Id { get; set; }
        public int CategoryId { get; set; }
        public string FrameType { get; set; } =string.Empty;

        public decimal FramePrice { get; set; }
        public string FrameSet { get; set; }=string.Empty;

        public string Name { get; set; }

        public string SvgPath { get; set; } 

        public bool? IsCharacter { get; set; }

        public int? DisplayOrder { get; set; }

        public string Thumbnail { get; set; }
        public string CategoryType { get; set; }
        public bool IsActive { get; set; }
    }

    //public class FrameFile
    //{
    //    public string Name { get; set; }
    //    public string SvgPath { get; set; }
    //    public bool? IsCharacter { get; set; } // only for kids-space
    //}


}