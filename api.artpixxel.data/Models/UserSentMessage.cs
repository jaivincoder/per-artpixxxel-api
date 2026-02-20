using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.artpixxel.data.Models
{
  public  class UserSentMessage :  AuditedDeletableEntity
    {


        public string UserId { get; set; }
        public User User { get; set; }
      
        public string MessageId { get; set; }
        [ForeignKey(nameof(MessageId))]
        public Message Message { get; set; }
        public bool SenderDeleted { get; set; }


    }
}
