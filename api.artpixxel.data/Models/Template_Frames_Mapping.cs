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
    public class Template_Frames_Mapping : AuditedDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TemplateConfigId { get; set; }

        [ForeignKey(nameof(TemplateConfigId))]
        public Template_Configs TemplateConfig { get; set; }

        [Required]
        public int OrderNo { get; set; }

        [Required]
        public string FrameId { get; set; }

        [ForeignKey(nameof(FrameId))]
        public Frame Frames { get; set; }
        public bool IsActive { get; set; }
    }
}
