

using api.artpixxel.data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace api.artpixxel.data.Models
{
    public class SMTP : AuditedDeletableEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string SenderName { get; set; }
        public SMTPState SMTPState { get; set; }
        public string Description { get; set; }
    }
}
