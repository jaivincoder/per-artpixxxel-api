
using System;

namespace api.artpixxel.data.Models.Base
{
   public interface IAuditedDeletableEntity
    {
        DateTime? DeletedOn { get; set; }
        string DeletedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
