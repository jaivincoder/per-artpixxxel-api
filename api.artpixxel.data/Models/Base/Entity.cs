
using System;

namespace api.artpixxel.data.Models.Base
{
   public class Entity: IEntity
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
       
    }
}
