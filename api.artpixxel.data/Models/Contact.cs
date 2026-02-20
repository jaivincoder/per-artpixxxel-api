using api.artpixxel.data.Models.Base;


namespace api.artpixxel.data.Models
{
    public class Contact : AuditedDeletableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string IPAddress { get; set; }
    }
}
