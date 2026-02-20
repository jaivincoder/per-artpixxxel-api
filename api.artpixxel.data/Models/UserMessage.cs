using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
  public  class UserMessage : AuditedDeletableEntity
    {
        public User User { get; set; }
        public string UserId { get; set; }
        public string MessageId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public Message Message { get; set; }

        public ReadStatus ReadStatus { get; set; }
        public bool Delivered { get; set; }
    }
}
