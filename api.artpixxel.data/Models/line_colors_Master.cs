using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace api.artpixxel.data.Models
{
    public class line_colors_Master : AuditedDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, MaxLength(10)]
        public string Color { get; set; } = string.Empty;

        [ForeignKey(nameof(CategoryId))]
        public frame_categories Category { get; set; }
        public bool IsActive { get; set; }
    }
}
