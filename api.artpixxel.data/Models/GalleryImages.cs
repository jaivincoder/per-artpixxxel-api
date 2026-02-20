using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace api.artpixxel.data.Models
{
    public class GalleryImages : AuditedDeletableEntity
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public frame_categories Category { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string ImageName { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

  
}
