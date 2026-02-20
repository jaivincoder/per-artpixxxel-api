

using System;

namespace api.artpixxel.data.Models.Base
{
    public class AuditedDeletableEntity : Entity, IAuditedDeletableEntity
    {
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
