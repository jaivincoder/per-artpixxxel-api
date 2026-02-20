

using System.Collections.Generic;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Models;

namespace api.artpixxel.data.Features.SMTPs
{
    public class SMTPModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string NewSMTPPassword { get; set; }
        public string SenderName { get; set; }
        public string SMTPState { get; set; }
        public string Description { get; set; }
    }

    public class SMTPPassCheck
    {
        public string SMTPId { get; set; }
        public string SMTPCurrentPassword { get; set; }
    }

    public class SMTPCheck
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class EmailTemplate
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Body { get; set; }
        public bool ActionButton { get; set; }
        public string ActionCommand { get; set; }
        public string URL { get; set; }
        public bool HasHeroImage { get; set; }
        public EmailType EmailType { get; set; }
    }

    public class EmailSendStatus
    {
        public string Message { get; set; }
        public bool Succeeded { get; set; }
    }


    public enum EmailType
    {
        Notification = 0,
        Welcome = 1,
        Announcement = 2,
        Newsletter = 3,
        Promotional = 4,
        Error = 6,
        PasswordChange = 7,
        SMTPTest = 10,
        Order = 11

    }

    public class EmailModel
    {
        public SMTP SMTP { get; set; }
        public EmailTemplate Body { get; set; }
        public List<string> Emails { get; set; }
        public string Subject { get; set; }
    }


    public class SMTPInit
    {
        public List<SMTPModel> SMTPs { get; set; }

    }


    public class SMTPCRUDResponse : SMTPInit
    {
        public BaseResponse Response { get; set; }

    }

    public class SMTPWriteRequest
    {
        public Pagination Pagination { get; set; }
        public SMTPModel SMTP { get; set; }
    }


    public class SMTPDeleteRequest
    {
        public Pagination Pagination { get; set; }
        public string Id { get; set; }
    }


}