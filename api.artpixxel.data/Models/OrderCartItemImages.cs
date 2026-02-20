using api.artpixxel.data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static api.artpixxel.data.Models.Permissions;

namespace api.artpixxel.data.Models
{
    public class OrderCartItemImages : AuditedDeletableEntity
    {
        [Key]
        public int Id { get; set; }
        public int OrderCartItemId { get; set; }
        [ForeignKey(nameof(OrderCartItemId))]
        public OrderCartItems OrderCartItems { get; set; }
        public string CroppedItemImage { get; set; }
        public string OriginalItemImage { get; set; }
    }
}
