

using api.artpixxel.data.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace api.artpixxel.data.Models
{
   public class Message : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentRelPath { get; set; }
        public string AttachmentAbsPath { get; set; }
        public string AttachmentTitle { get; set; }
        public FileType? AttachmentFileType { get; set; }
        public ICollection<UserMessage> UserMessages { get; } = new List<UserMessage>();
        public ICollection<UserSentMessage> UserSentMessages { get; } = new List<UserSentMessage>();
        [NotMapped]
        public IEnumerable<Message> Recipients => UserMessages.Select(e => e.Message);
        [NotMapped]
        public IEnumerable<User> Senders => UserSentMessages.Select(e => e.User);
    }
}
