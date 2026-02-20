
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.SMTPs;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Extensions;
using api.artpixxel.service.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using api.artpixxel.data.Features.Accounts;
using api.artpixxel.repo.Extensions.Models;
using api.artpixxel.data.Features.Checkouts;
using api.artpixxel.data.Features.OrderStatuses;
using api.artpixxel.repo.Utils.Generator;
using api.artpixxel.repo.Utils.Generator.Config;
using api.artpixxel.data.Features.Contacts;
using MailKit.Security;
using System.Text;

namespace api.artpixxel.repo.Features.SMTPs
{
   public class SMTPService : ISMTPService
    {

        private readonly UserManager<User> _userManager;
        private readonly ArtPixxelContext _context;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly AppKeyConfig _appkeys;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public SMTPService(
             UserManager<User> userManager,
            IOptions<AppKeyConfig> appkeys,
            RoleManager<UserRole> roleManager,
            ICurrentUserService currentUserService,
            IWebHostEnvironment hostingEnvironment,
            ArtPixxelContext context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
            _roleManager = roleManager;
            _userManager = userManager;
            _appkeys = appkeys.Value;
        }




        private async Task<EmailSendStatus> SendConfigValidationMail(SMTPModel request)
        {
            try
            {
                string toEmailAddress = string.Empty;

                if (await _context.SMTPs.AnyAsync(e => e.Id == request.Id))
                {
                    SMTP sMTP = await _context.SMTPs.FindAsync(request.Id);
                    if (sMTP != null)
                    {
                    User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                    if (user != null)
                    {


                            toEmailAddress = user.Email;

                            //toEmailAddress = DefaultEmails.SuperAdminEmail;


                        if (!string.IsNullOrEmpty(toEmailAddress))
                        {
                            SMTP newsMTP = new()
                            {
                                Name = request.Name,
                                SMTPUserName = request.SMTPUserName,
                                SMTPServer = request.SMTPServer,
                                SMTPPort = request.SMTPPort,
                                SMTPPassword = sMTP.SMTPPassword.Decrypt(_appkeys.EncryptionKey),
                                SenderName = request.SenderName,

                            };




                            string emailBody = "<p style='font-size: 14px; margin: 1px 0px 0px 0px; padding: 0;'><span style='font-size: 12px;'>SMTP Configuration Valid</span></p>";





                            EmailTemplate emailTemplate = new()
                            {
                                ActionButton = false,
                                URL = _currentUserService.GetBaseURl(),
                                SubTitle = "SMTP Validation Mail",
                                HasHeroImage = true,
                                Body = emailBody,
                                EmailType = EmailType.SMTPTest,
                                Title = "SMTP Validation",
                            };

                            EmailModel model = new EmailModel
                            {
                                Body = emailTemplate,
                                Emails = new List<string> { toEmailAddress },
                                SMTP = newsMTP,
                                Subject = "SMTP Validation",

                            };




                            MimeMessage message = new MimeMessage();
                            BodyBuilder builder = new BodyBuilder();
                            InternetAddressList EmailList = new InternetAddressList();

                            foreach (string email in model.Emails)
                            {

                                EmailList.Add(MailboxAddress.Parse(email));
                            }

                            builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                            message.To.AddRange(EmailList);
                            message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                            message.Subject = model.Subject;
                            message.Date = DateTime.Now;
                            message.Body = builder.ToMessageBody();

                            return await SendValidateMail(message, model.SMTP);


                        }


                        return new EmailSendStatus
                        {
                            Message = "Your email wasn't found. Test email couldn't be sent.",
                            Succeeded = false
                        };


                    }





                    return new EmailSendStatus
                    {
                        Message = "Current user not found.",
                        Succeeded = false
                    };
                }

            }

                return new EmailSendStatus
                {
                    Message = "SMTP user not found.",
                    Succeeded = false
                };



            }
            catch (Exception ex)
            {

                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false
                };
            }
        }


        private async Task<EmailSendStatus> SendNewPasswordValidationMail(SMTPModel request)
        {
            try
            {

                if(await _context.SMTPs.AnyAsync(e => e.Id == request.Id))
                {
                    SMTP sMTP = await _context.SMTPs.FindAsync(request.Id);

                    if(sMTP != null)
                    {
                        if(request.SMTPPassword != sMTP.SMTPPassword.Decrypt(_appkeys.EncryptionKey))
                        {
                            return new EmailSendStatus
                            {
                                Message = "Supplied current SMTP password does not match the actual current password.",
                                Succeeded = false
                            };
                        }


                        else if(request.NewSMTPPassword == sMTP.SMTPPassword.Decrypt(_appkeys.EncryptionKey))
                        {
                            return new EmailSendStatus
                            {
                                Message = "New password same as current password.",
                                Succeeded = false
                            };
                        }

                        else
                        {
                            string toEmailAddress = string.Empty;
                            User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                            if (user != null)
                            {


                                toEmailAddress =  user.Email;  // DefaultEmails.SuperAdminEmail;



                                if (!string.IsNullOrEmpty(toEmailAddress))
                                {

                                    sMTP.Name = request.Name;
                                    sMTP.SMTPUserName = request.SMTPUserName;
                                    sMTP.SMTPServer = request.SMTPServer;
                                    sMTP.SMTPPort = request.SMTPPort;
                                    sMTP.SMTPPassword = request.NewSMTPPassword;
                                    sMTP.SenderName = request.SenderName;

                                  
                                    string emailBody = "<p style='font-size: 14px; margin: 1px 0px 0px 0px; padding: 0;'><span style='font-size: 12px;'>SMTP Configuration Valid</p>";





                                    EmailTemplate emailTemplate = new()
                                    {
                                        ActionButton = false,
                                        URL = _currentUserService.GetBaseURl(),
                                        SubTitle = "SMTP Validation Mail",
                                        HasHeroImage = true,
                                        Body = emailBody,
                                        EmailType = EmailType.SMTPTest,
                                        Title = "SMTP Validation",
                                    };

                                    EmailModel model = new()
                                    {
                                        Body = emailTemplate,
                                        Emails = new List<string> { toEmailAddress },
                                        SMTP = sMTP,
                                        Subject = "SMTP Validation",

                                    };




                                    MimeMessage message = new MimeMessage();
                                    BodyBuilder builder = new BodyBuilder();
                                    InternetAddressList EmailList = new InternetAddressList();

                                    foreach (string email in model.Emails)
                                    {

                                        EmailList.Add(MailboxAddress.Parse(email));
                                    }

                                    builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                                    message.To.AddRange(EmailList);
                                    message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                                    message.Subject = model.Subject;
                                    message.Date = DateTime.Now;
                                    message.Body = builder.ToMessageBody();

                                    return await SendValidateMail(message, model.SMTP);


                                }


                                return new EmailSendStatus
                                {
                                    Message = "Your email wasn't found. Test email couldn't be sent.",
                                    Succeeded = false
                                };


                            }





                            return new EmailSendStatus
                            {
                                Message = "Current user not found.",
                                Succeeded = false
                            };
                        }
                    }
                }


                return new EmailSendStatus
                {
                    Message = "SMTP configuration not found. This configuration may have been deleted.",
                    Succeeded = false
                };



            }
            catch (Exception ex)
            {

                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false
                };
            }
        }

        private async Task<EmailSendStatus> SendValidationMail(SMTPModel request)
        {
            try
            {
                string toEmailAddress = string.Empty;
                User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                if (user != null)
                {
                   

                        toEmailAddress = user.Email;
                    


                    if (!string.IsNullOrEmpty(toEmailAddress))
                    {
                        SMTP sMTP = new()
                        {
                            Name = request.Name,
                            SMTPUserName = request.SMTPUserName,
                            SMTPServer = request.SMTPServer,
                            SMTPPort = request.SMTPPort,
                            SMTPPassword = request.SMTPPassword,
                            SenderName = request.SenderName,

                        };




                        string emailBody = "<p style='font-size: 14px; margin: 1px 0px 0px 0px; padding: 0;'><span style='font-size: 12px;'>Valid SMTP configuration</p>";





                        EmailTemplate emailTemplate = new EmailTemplate
                        {
                            ActionButton = false,
                            URL = _currentUserService.GetBaseURl(),
                            SubTitle = "SMTP Validation Mail",
                            HasHeroImage = true,
                            Body = emailBody,
                            EmailType = EmailType.SMTPTest,
                            Title = "SMTP Validation",
                        };

                        EmailModel model = new EmailModel
                        {
                            Body = emailTemplate,
                            Emails = new List<string> { toEmailAddress },
                            SMTP = sMTP,
                            Subject = "SMTP Validation",

                        };




                        MimeMessage message = new MimeMessage();
                        BodyBuilder builder = new BodyBuilder();
                        InternetAddressList EmailList = new InternetAddressList();

                        foreach (string email in model.Emails)
                        {

                            EmailList.Add(MailboxAddress.Parse(email));
                        }

                        builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                        message.To.AddRange(EmailList);
                        message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                        message.Subject = model.Subject;
                        message.Date = DateTime.Now;
                        message.Body = builder.ToMessageBody();

                        return await SendValidateMail(message, model.SMTP);


                    }


                    return new EmailSendStatus
                    {
                        Message = "Your email wasn't found. Test email couldn't be sent.",
                        Succeeded = false
                    };


                }





                return new EmailSendStatus
                {
                    Message = "Current user not found.",
                    Succeeded = false
                };



            }
            catch (Exception ex)
            {

                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false
                };
            }
        }





        private async Task<EmailSendStatus> SendValidateMail(MimeMessage message, SMTP sMTP)
        {
            

                using (var client = new SmtpClient())
                {
                    try
                    {
                        
                        client.CheckCertificateRevocation = false;
                        await client.ConnectAsync(sMTP.SMTPServer, Convert.ToInt32(sMTP.SMTPPort), SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(sMTP.SMTPUserName, sMTP.SMTPPassword);
                        await client.SendAsync(message);
                      
                    }
                    catch (Exception ex)
                    {

                        return new EmailSendStatus
                        {
                            Message = ex.Message,
                            Succeeded = false
                        };
                    }

                    finally
                    {
                        await client.DisconnectAsync(true);
                        client.Dispose();


                    }

                  
                }

                return new EmailSendStatus
                {
                    Message = string.Empty,
                    Succeeded = true
                };

            
          
        }






        private async Task<EmailSendStatus> EmailSender(MimeMessage message, SMTP sMTP)
        {
           

                using (var client = new SmtpClient())
                {

                    try
                    {
                        client.CheckCertificateRevocation = false;
                        await client.ConnectAsync(sMTP.SMTPServer, Convert.ToInt32(sMTP.SMTPPort), SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(sMTP.SMTPUserName, sMTP.SMTPPassword.Decrypt(_appkeys.EncryptionKey));
                        await client.SendAsync(message);
                        
                    }
                    catch (Exception ex)
                    {

                        return new EmailSendStatus
                        {
                            Message = ex.Message,
                            Succeeded = false
                        };
                    }

                    finally
                    {
                       await client.DisconnectAsync(true);
                        client.Dispose();
                    }
                }

                return new EmailSendStatus
                {
                    Message = "and email(s) successfully dispatched to this effect.",
                    Succeeded = true
                };

            
          
        }



        private async Task<string> ComposeHTMLBody(BodyBuilder builder, EmailTemplate emailTemplate, CheckoutNotification checkoutNotification = null)
        {

           
            string LogoImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\logo.png";
            FileContentInfo logoContent = await LogoImg.FileContent();
            MimeEntity logoThumb = builder.LinkedResources.Add(logoContent.FileName, logoContent.Bytes, logoContent.ContentType);
            logoThumb.ContentId = "artpixxelLogo";

            MimeEntity artworkThumb = null;

            string InstagramImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\instagram.png";
            FileContentInfo InstagramImgContent = await InstagramImg.FileContent();
            MimeEntity InstagramThumb = builder.LinkedResources.Add(InstagramImgContent.FileName, InstagramImgContent.Bytes, InstagramImgContent.ContentType);
            InstagramThumb.ContentId = "Instagram";

            string TwitterImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\twitter.png";
            FileContentInfo TwitterImgContent = await TwitterImg.FileContent();
            MimeEntity TwitterThumb = builder.LinkedResources.Add(TwitterImgContent.FileName, TwitterImgContent.Bytes, TwitterImgContent.ContentType);
            TwitterThumb.ContentId = "Twitter";


            string FacebookImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\facebook.png";
            FileContentInfo FacebookImgContent = await FacebookImg.FileContent();
            MimeEntity FacebookThumb = builder.LinkedResources.Add(FacebookImgContent.FileName, FacebookImgContent.Bytes, FacebookImgContent.ContentType);
            FacebookThumb.ContentId = "Facebook";


            string LinkedInImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\linkedin.png";
            FileContentInfo LinkedInImgContent = await LinkedInImg.FileContent();
            MimeEntity LinkedInThumb = builder.LinkedResources.Add(LinkedInImgContent.FileName, LinkedInImgContent.Bytes, LinkedInImgContent.ContentType);
            LinkedInThumb.ContentId = "LinkedIn";


            if(emailTemplate.HasHeroImage == true)
            {
                if (emailTemplate.EmailType == EmailType.Notification)
                {
                    string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\notification.png";
                    FileContentInfo ArtworkImgContent = await ArtworkImg.FileContent();
                    artworkThumb = builder.LinkedResources.Add(ArtworkImgContent.FileName, ArtworkImgContent.Bytes, ArtworkImgContent.ContentType);
                    artworkThumb.ContentId = "Notification";
                }

                else if (emailTemplate.EmailType == EmailType.PasswordChange)
                {
                    string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\password.png";
                    FileContentInfo ArtworkImgContent = await ArtworkImg.FileContent();
                    artworkThumb = builder.LinkedResources.Add(ArtworkImgContent.FileName, ArtworkImgContent.Bytes, ArtworkImgContent.ContentType);
                    artworkThumb.ContentId = "Password";
                }


                else if (emailTemplate.EmailType == EmailType.SMTPTest)
                {
                    string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\smtptest.png";
                    FileContentInfo ArtworkImgContent = await ArtworkImg.FileContent();
                    artworkThumb = builder.LinkedResources.Add(ArtworkImgContent.FileName, ArtworkImgContent.Bytes, ArtworkImgContent.ContentType);
                    artworkThumb.ContentId = "SMTP";
                }

                else if (emailTemplate.EmailType == EmailType.Error)
                {
                    string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\error.png";
                    FileContentInfo ArtworkImgContent = await ArtworkImg.FileContent();
                    artworkThumb = builder.LinkedResources.Add(ArtworkImgContent.FileName, ArtworkImgContent.Bytes, ArtworkImgContent.ContentType);
                    artworkThumb.ContentId = "Error";
                }

                else if (emailTemplate.EmailType == EmailType.Welcome)
                {
                    string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\welcome.png";
                    FileContentInfo ArtworkImgContent = await ArtworkImg.FileContent();
                    artworkThumb = builder.LinkedResources.Add(ArtworkImgContent.FileName, ArtworkImgContent.Bytes, ArtworkImgContent.ContentType);
                    artworkThumb.ContentId = "Welcome";
                }
            }

            

           

           
            string container = "<!DOCTYPE html><html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:o='urn:schemas-microsoft-com:office:office'>";
            container += "<head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>";
            container += "<meta name='x-apple-disable-message-reformatting'><title></title>";
            container += "<!--[if mso]><style>.ol{width:100%; }table { border-collapse:collapse; border-spacing:0; border: none; margin: 0; }div, td { padding: 0; }div { margin: 0!important; }";
            container += "</style><noscript><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml></noscript><![endif]-->";
            container += "<style>table, td, div, h1, p {font-family: Arial, sans-serif; font-size:10px}@media screen and(max-width: 530px) {.unsub {display: block;padding: 8px;";
            container += "margin-top: 14px;border-radius: 6px;background-color: #555555;text-decoration: none!important;font-weight: bold; }.col-lge {max-width:100% !important;}";
            container += ".res-td{width: 100% !important;}}@media screen and(min-width: 531px) {.col-sml {max-width: 27% !important;}.col-lge {max-width: 73 % !important;}}";
            container += ".container600 {width: 600px;max-width: 100%;}@media all and(max-width: 600px){.container600 {width: 100% !important;}} .col100 {width: 100%;}";
            container += ".col49 {width: 49%;}.col2{width:2%;}.col50{width:50%;}@media all and (max-width: 599px) {.fluid {width:100% !important;}.reorder {width: 100% !important;margin: 0 auto 10px;}";
            container += ".ghost-column {display: none;height: 0;width: 0;overflow: hidden;max-height:0;max-width:0;}} </style></head>";
            container += "<body style = 'margin:0;padding:0;word-spacing:normal;background-color:#f1f2f4;'> <div role='article' aria-roledescription='email' lang ='en'";
            container += "-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;background-color:#f1f2f4;'><table role='presentation' style ='width:100%;border:none;border-spacing:0;'>";
            container += "<tr><td align='center' style = 'padding:0;'><!--[if mso]><table role='presentation' align='center' style='width:600px;'>";
            container += "<tr><td><![endif]--><table role='presentation' style = 'width:94%;max-width:600px;border:none;border-spacing:0;text-align:left;font-family:Arial,sans-serif;font-size:16px;line-height:22px;color:#363636;'>";
            container += "<tr><td style='padding:40px 30px 30px 30px;text-align:center;font-size:24px;font-weight:bold;'><a href ='http://artpixxel.com/' style='text-decoration:none;'>";
            container += "<img align='center' border='0' src='cid:" + logoThumb.ContentId + "' alt='Logo' title='Image'  width='165'  style = 'width:165px;max-width:80%;height:auto;border:none;text-decoration:none;color:#ffffff;'></a>";
            container += "</td></tr><tr><td style ='padding:30px;background-color:#ffffff;'>";
            if (!string.IsNullOrEmpty(emailTemplate.Title))
            {
                container += "<h1 style='margin-top:0;margin-bottom:16px;font-size:26px;line-height:32px;font-weight:bold;letter-spacing:-0.02em; text-align: center;'>" + emailTemplate.Title + "</h1>";
            }
           
            if (!string.IsNullOrEmpty(emailTemplate.SubTitle))
            {
                container += "<p style='margin-top:0; margin-bottom:16px; font-size:18px; text-align: center; color: #777777;'>" + emailTemplate.SubTitle + "</p>";
            }
          
            container += "</td></tr>";
            if (emailTemplate.HasHeroImage == true && (artworkThumb != null))
            {
                container += "<tr><td style = 'padding:0;line-height:28px;font-weight:bold;background-color:#ffffff;'>";
                container += "<img align='center' border='0'  src='cid:" + artworkThumb.ContentId + "' alt='Image' title='Image' width ='600' ";
                container +=  "alt = 'image' style='-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 600px;'></td></tr>";


            }

            container += "<tr><td style='padding:30px;background-color:#ffffff; font-size: 12px'>";
            container += emailTemplate.Body;
            if(checkoutNotification != null)
            {
                container += await OrderDetails(checkoutNotification, builder);
            }
            container += "</td></tr>";
            container += "<tr><td style = 'padding:30px;text-align:center;font-size:12px;background-color:#e2e3e9;color:#666b85;'>";
            container +="<p style = 'margin:0 0 8px 0;'><a href = 'https://www.fb.me/artpixxel' style='text-decoration:none;'>";
            container += "<img src='cid:" + FacebookThumb.ContentId + "' alt='facebook' title='facebook' width='20' height='20' alt='f' style='display:inline-block;color:#666b85;'></a>";
            container += "<a href = 'https://www.twitter.com/artpixxel' style = 'text-decoration:none;'><img src='cid:" + TwitterThumb.ContentId + "' alt='Twitter' title='Twitter' width='20' height='20' alt='t' style='display:inline-block;color:#666b85;'></a>";
            container += "<a href='https://www.instagram.com/artpixxel' style='text-decoration:none;'><img src='cid:" + InstagramThumb.ContentId + "' alt='Instagram' title='Instagram' width='20' height='20' alt ='t' style ='display:inline-block;color:#666b85;'></a>";
            container += "<a href='http://www.linkedin.com/company/artpixxel' style = 'text-decoration:none;'><img src='cid:" + LinkedInThumb.ContentId + "' title='LinkedIn' width = '20' height ='20' alt ='linkedin' style ='display:inline-block;color:#666b85;'></a></p>";
            container += "<p style='margin:0;font-size:14px;line-height:20px;'> Copyright &copy; " + DateTime.Now.Year + ". ArtPixxel<br></p>";
            container += "</td></tr></table><!--[if mso]></td></tr></table><![endif]--></td></tr></table></div></body></html>";

            return container;
        }

        private string Compose(BodyBuilder builder, EmailTemplate emailTemplate)
        {

            string LogoImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\logo.png";
            var logoThumb = builder.LinkedResources.Add("logo.png", File.ReadAllBytes(LogoImg), new ContentType("image/png", "png"));
            logoThumb.ContentId = "aslLogo";

            MimeEntity artworkThumb;

            if (emailTemplate.EmailType == EmailType.Notification)
            {
                string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\notification.png";
                artworkThumb = builder.LinkedResources.Add("notification.png", File.ReadAllBytes(ArtworkImg), new ContentType("image/png", "png"));
                artworkThumb.ContentId = "Notification";
            }
         
            else if (emailTemplate.EmailType == EmailType.PasswordChange)
            {
                string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\password.png";
                artworkThumb = builder.LinkedResources.Add("password.png", File.ReadAllBytes(ArtworkImg), new ContentType("image/png", "png"));
                artworkThumb.ContentId = "Password";
            }
          

            else if (emailTemplate.EmailType == EmailType.SMTPTest)
            {
                string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\smtptest.png";
                artworkThumb = builder.LinkedResources.Add("smtp.png", File.ReadAllBytes(ArtworkImg), new ContentType("image/png", "png"));
                artworkThumb.ContentId = "SMTP";
            }
        
            else if (emailTemplate.EmailType == EmailType.Error)
            {
                string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\error.png";
                artworkThumb = builder.LinkedResources.Add("error.png", File.ReadAllBytes(ArtworkImg), new ContentType("image/png", "png"));
                artworkThumb.ContentId = "Error";
            }

            else if (emailTemplate.EmailType == EmailType.Welcome)
            {
                string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\welcome.png";
                artworkThumb = builder.LinkedResources.Add("welcome.png", File.ReadAllBytes(ArtworkImg), new ContentType("image/png", "png"));
                artworkThumb.ContentId = "Welcome";
            }

            else
            {
                string ArtworkImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\notification.png";
                artworkThumb = builder.LinkedResources.Add("notification.png", File.ReadAllBytes(ArtworkImg), new ContentType("image/png", "png"));
                artworkThumb.ContentId = "Notification";
            }




            string container = "<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>";
            container += "<html xmlns ='http://www.w3.org/1999/xhtml' xmlns: v='urn:schemas-microsoft-com:vml' xmlns: o='urn:schemas-microsoft-com:office:office'>";
            container += "<head><!--[if gte mso 9]><xml><o:OfficeDocumentSettings><o:AllowPNG/><o:PixelsPerInch>96</o:PixelsPerInch>";
            container += "</o:OfficeDocumentSettings></xml><![endif]-->";
            container += "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>";
            container += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
            container += "<meta name='x-apple-disable-message-reformatting'>";
            container += "<!--[if !mso]><!--><meta http-equiv='X-UA-Compatible' content='IE=edge'><!--<![endif]-->";
            container += "<title></title><style type='text/css'>";
            container += "table, td {color: #000000; } a { color: #e4007d; text-decoration: underline; } @media (max-width: 480px) { #u_content_text_2 .v-text-align { text-align: center !important; } #u_content_image_2 .v-src-width { width: auto !important; } #u_content_image_2 .v-src-max-width { max-width: 60% !important; } #u_content_image_3 .v-src-width { width: auto !important; } #u_content_image_3 .v-src-max-width { max-width: 60% !important; } #u_content_image_4 .v-src-width { width: auto !important; } #u_content_image_4 .v-src-max-width { max-width: 60% !important;}}";
            container += "@media only screen and(min-width: 620px) {.u-row {width: 600px!important;}.u-row.u-col {vertical-align: top;}";
            container += ".u-row.u-col-33p33 {width: 199.98px!important;}.u-row.u-col-50{width: 300px!important;}.u-row.u-col-30 {width: 180px!important;}";
            container += ".u-row.u-col-70 {width: 420px!important;}.u-row.u-col-100 {width: 600px!important;}}";
            container += "@media(max-width: 620px) {.u-row-container {max-width: 100% !important;padding-left: 0px!important;padding-right: 0px!important;}.u-row.u-col{";
            container += "min-width: 320px!important;max-width: 100% !important;display: block!important;}.u-row {width: calc(100%-40px)!important;}.u-col{width: 100% !important;}";
            container += ".u-col > div {margin: 0 auto;}}body {margin: 0;padding: 0;}table,tr,td {vertical-align: top;border-collapse: collapse;}";
            container += "p {margin: 0;}.ie-container table,.mso-container table {table-layout: fixed;}* {line-height: inherit;}a[x-apple-data-detectors = 'true'] {";
            container += "color: inherit!important;text-decoration: none!important;}</style>";
            container += "<!--[if !mso]><!--><link href='https://fonts.googleapis.com/css?family=Crimson+Text:400,700&display=swap' rel='stylesheet' type = 'text/css' ><link href = 'https://fonts.googleapis.com/css?family=Lato:400,700&display=swap' rel = 'stylesheet' type='text/css'><link href = 'https://fonts.googleapis.com/css?family=Montserrat:400,700&display=swap' rel = 'stylesheet' type = 'text/css'><link href = 'https://fonts.googleapis.com/css?family=Playfair+Display:400,700&display=swap' rel = 'stylesheet' type = 'text/css'><!--<![endif]-->";
            container += "</head><body class='clean-body u_body' style='margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #eaeef1;color: #000000'>";
            container += "<!--[if IE]><div class='ie-container'><![endif]--><!--[if mso]><div class='mso-container'><![endif]-->";
            container += "<table style = 'border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #eaeef1;width:100%' cellpadding='0' cellspacing='0'>";
            container += "<tbody><tr style = 'vertical-align: top'><td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>";
            container += "<!--[if (mso)|(IE)]><table width = '100%' cellpadding='0' cellspacing='0' border='0'><tr><td align = 'center' style='background-color: #eaeef1;'><![endif]-->";
            container += "<div class='u-row-container' style='padding: 35px;'><div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;'>";
            container += "<div style = 'border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>";
            container += "<!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style = 'padding: 0px;background-color: #eaeef1;' align='center'><table cellpadding = '0' cellspacing='0' border='0' style='width:600px;'><tr style = 'background-color:#eaeef1;'><![endif]-->";
            container += "<!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->";
            container += "<div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>";
            container += "<div style = 'width: 100% !important;'><!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->";
            container += "<table style = 'font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            container += "<tbody><tr><td style='overflow-wrap:break-word;word-break:break-word;padding:30px 10px 15px;font-family:'Montserrat',sans-serif;' align='left'>";
            container += "<table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td class='v-text-align' style='padding-right: 0px;padding-left: 0px;' align='center'>";
            container += "<img align = 'center' border='0' src='cid:" + logoThumb.ContentId + "' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 45%;max-width: 122.4px;' width='122.4' class='v-src-width v-src-max-width'/>";
            container += "</td></tr></table></td></tr></tbody></table><!--[if (!mso)&(!IE)]><!--></div><!--<![endif]--></div></div><!--[if (mso)|(IE)]></td><![endif]-->";
            container += "<!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]--></div></div></div>";

            // EMAIL BODY

            // centered image

            container += "<div class='u-row-container' style='padding: 0px;background-color: transparent'>";
            container += "<div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>";
            container += "<div style = 'border-collapse: collapse;display: table;width: 100%;background-color: transparent;' >";
            container += "<!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding = '0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #f9f9f9;'><![endif]-->";
            container += "<!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->";
            container += "<div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>";
            container += "<div style='width: 100% !important;'> <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->";
            container += "<table style='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            container += "<tbody><tr><td style = 'overflow-wrap:break-word;word-break:break-word;padding:0 0 25px 0 0px 0px;font-family:'Montserrat',sans-serif;' align='left'>";
            container += "<table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td class='v-text-align' style='padding-right: 0px;padding-left: 0px;' align='center'>";
            container += "<img align = 'center' border='0'  src='cid:" + artworkThumb.ContentId + "' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 600px;' width='600' class='v-src-width v-src-max-width'/>";
            container += "</td></tr></table></td></tr> </tbody></table><!--[if (!mso)&(!IE)]><!--></div><!--<![endif]--></div></div><!--[if (mso)|(IE)]></td><![endif]-->";
            container += "<!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]--></div></div></div>";


            //main content after the image
            container += "<div class='u-row-container' style='padding: 0px;background-color: transparent'>";
            container += "<div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #E4007D;'>";
            container += "<div style = 'border-collapse: collapse;display: table;width: 100%;background-color: transparent;' >";
            container += "<!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style = 'padding: 0px;background-color: transparent;' align='center'><table cellpadding = '0' cellspacing='0' border='0' style='width:600px;'><tr style = 'background-color: #E4007D;'><![endif]-->";
            container += "<!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->";
            container += "<div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>";
            container += "<div style='width: 100% !important;'>";
            container += " <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->";
            container += "<table style='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            container += "<tbody><tr><td style ='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Montserrat',sans-serif;' align='left'>";
            container += "</td></tr></tbody></table><table style ='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            container += "<tbody><tr><td style = 'overflow-wrap:break-word;word-break:break-word;padding:10px 10px 20px;font-family:'Montserrat',sans-serif;' align='left'>";
            container += "<div class='v-text-align' style='color: #ffffff; line-height: 140%; text-align: center; word-wrap: break-word;'>";
            container += "<p style='font-size: 14px; line-height: 140%;'><span style='font-size: 16px; line-height: 22.4px;'>" + emailTemplate.Title + "</span></p>";
            container += "</div></td></tr></tbody></table><!--[if (!mso)&(!IE)]><!--></div><!--<![endif]--></div></div><!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->";
            container += "</div></div></div>";

            container += "<div class='u-row-container' style='padding: 0px;background-color: transparent'>";
            container += "<div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>";
            container += "<div style = 'border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>";
            container += "<!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style = 'padding: 0px;background-color: transparent;' align='center'><table cellpadding = '0' cellspacing='0' border='0' style='width:600px;'><tr style = 'background-color: #f9f9f9;'><![endif]-->";
            container += "<!--[if (mso)|(IE)]><td align='center' width='300' style='width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->";
            container += "<div class='u-col u-col-50' style='max-width: 180px;min-width: 150px;display: table-cell;vertical-align: top;'>";
            container += "<div style = 'width: 100% !important;'><!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->";
            container += "<table style='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            container += "<tbody><tr><td style = 'overflow-wrap:break-word;word-break:break-word;padding:10px 10px 0px 30px;font-family:'Montserrat',sans-serif;' align='left'>";
            container += "<div class='v-text-align' style='color: #727171; line-height: 140%; text-align: left; word-wrap: break-word;'>";
            container += "<p style = 'font-size: 14px; line-height: 140%; padding-left:15px; padding-right:15px'><strong><span style='font-size: 18px; line-height: 25.2px;'>" + emailTemplate.SubTitle + "</span></strong></p>";
            container += "</div> </td></tr></tbody></table><table style='font-family:'Montserrat',sans-serif;' role= 'presentation' cellpadding= '0' cellspacing= '0' width= '100%' border= '0'>";
            container += "<tbody><tr><td style= 'overflow-wrap:break-word;word-break:break-word;padding:0px 10px 10px 30px !important;font-family:'Montserrat',sans-serif;' align= 'left'>";
            container += "<div class='v-text-align' style='line-height: 150%; text-align: left; word-wrap: break-word; padding-left:15px; padding-right:15px '>";

            container += emailTemplate.Body;
            container += emailTemplate.ActionButton ? "<a style='background-color:#E4007D; position: relative;border: none;color: #ffffff;border-radius: 3px;padding: 6px 22px;text-align: center; width:" +
               "auto;max-width: 180px;text-decoration: none;display: block;margin:20px auto 20px auto;font-size: 16px;' href='" + emailTemplate.URL + "' target='_blank'>" + emailTemplate.ActionCommand + "</a>" : "";
            container += "</div></td></tr></tbody></table><!--[if (!mso)&(!IE)]><!--></div><!--<![endif]--></div></div>";
            container += "<!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]--></div></div></div>";

            container += EmailFooter(builder);
            container += "<!--[if (mso)|(IE)]></td></tr></table><![endif]--></td></tr></tbody></table><!--[if mso]></div><![endif]--><!--[if IE]></div><![endif]-->";
            container += "</body ></html>";
            return container;



        }


        private string EmailFooter(BodyBuilder builder)
        {

            string InstagramImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\instagram.png";
            var InstagramThumb = builder.LinkedResources.Add("instagram.png", File.ReadAllBytes(InstagramImg), new ContentType("image/png", "png"));
            InstagramThumb.ContentId = "Instagram";

            string TwitterImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\twitter.png";
            var TwitterThumb = builder.LinkedResources.Add("twitter.png", File.ReadAllBytes(TwitterImg), new ContentType("image/png", "png"));
            TwitterThumb.ContentId = "Twitter";


            string FacebookImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\facebook.png";
            var FacebookThumb = builder.LinkedResources.Add("facebook.png", File.ReadAllBytes(FacebookImg), new ContentType("image/png", "png"));
            FacebookThumb.ContentId = "Facebook";


            string LinkedInImg = _hostingEnvironment.WebRootPath + "\\images\\Mail\\linkedin.png";
            var LinkedInThumb = builder.LinkedResources.Add("facebook.png", File.ReadAllBytes(LinkedInImg), new ContentType("image/png", "png"));
            LinkedInThumb.ContentId = "LinkedIn";




            string footer = "<div class='u-row-container' style='padding: 30px 0px 0px  0px; background-color: #e1e5ea'>";
            footer += "<div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>";
            footer += "<div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>";
            footer += "<!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: #e1e5ea;' align='center'><table cellpadding='0' cellspacing ='0' border='0' style='width:600px;'><tr style='background-color:#f9f9f9;'><![endif]-->";
            footer += "<!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->";
            footer += "<div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>";
            footer += "<div style='width: 100% !important;'><!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->";
            footer += "<table style='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            footer += "<tbody><tr><td style='overflow-wrap:break-word;word-break:break-word;padding:20px;font-family:'Montserrat',sans-serif;' align='left'>";
            footer += "<table height='0px' align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 0px solid #e1e5ea;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>";
            footer += "<tbody><tr style='vertical-align: top'><td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>";
            footer += "<span>&#160;</span></td></tr> </tbody></table></td></tr></tbody></table><!--[if (!mso)&(!IE)]><!--></div><!--<![endif]--></div></div><!--[if (mso)|(IE)]></td><![endif]-->";
            footer += "<!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]--></div></div></div>";
            footer += "<div class='u-row-container' style='padding: 0px;background-color: #e1e5ea'>";
            footer += "<div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>";
            footer += "<div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>";
            footer += "<!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style ='padding: 0px;background-color: #e1e5ea;'";
            footer += "align='center'><table cellpadding = '0' cellspacing='0' border='0' style='width:600px;'><tr style = 'background-color: transparent;' ><![endif]-->";
            footer += "<!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent; border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->";
            footer += "<div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>";
            footer += "<div style='width: 100% !important;'>";
            footer += "<!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->";
            footer += "<table style='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>";
            footer += "<tbody><tr><td style ='overflow-wrap:break-word;word-break:break-word;padding:40px 10px 10px;font-family:'Montserrat',sans-serif; align='left'>";
            footer += "<div align='center'><div style='display: table; max-width:187px;'>";
            footer += "<!--[if (mso)|(IE)]><table width = '187' cellpadding='0' cellspacing='0' border='0'><tr><td style = 'border-collapse:collapse;' align='center'><table width = '100%' cellpadding='0' cellspacing='0' border='0' style='border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:187px;'><tr><![endif]-->";
            footer += "<!--[if (mso)|(IE)]><td width='22' style='width:22px; padding-right: 15px;' valign='top'><![endif]-->";
            footer += "<table align='left' border='0' cellspacing='0' cellpadding='0' width='22' height='22' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right:15px'>";
            footer += "<tbody><tr style='vertical-align: top'><td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>";
            footer += "<a href='https://www.linkedin.com/company/artpixxel' title='LinkedIn' target='_blank'>";
            footer += "<img src='cid:" + LinkedInThumb.ContentId + "' title='LinkedIn' width='22' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width:22px !important'>";
            footer += "</a></td></tr></tbody></table><!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]><td width = '32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->";
            footer += "<table align='left' border='0' cellspacing='0' cellpadding='0' width='22' height='22' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>";
            footer += "<tbody><tr style='vertical-align: top'><td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>";
            footer += "<a href='https://twitter.com/artpixxel' title='Twitter' target='_blank'>";
            footer += "<img src='cid:" + TwitterThumb.ContentId + "' alt='Twitter' title='Twitter' width='22' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 22px !important'>";
            footer += "</a></td></tr></tbody></table><!--[if (mso)|(IE)]></td><![endif]--> <!--[if (mso)|(IE)]><td width ='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->";
            footer += "<table align='left' border='0' cellspacing='0' cellpadding='0' width='22' height='22' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>";
            footer += "<tbody><tr style='vertical-align: top'><td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>";
            footer += "<a href='https://instagram.com/artpixxel' title='Instagram' target='_blank'>";
            footer += "<img src='cid:" + InstagramThumb.ContentId + "' alt='Instagram' title='Instagram' width='22' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 22px !important'>";
            footer += "</a></td></tr></tbody></table> <!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]><td width = '32' style='width:32px; padding-right: 0px;' valign='top'><![endif]-->";
            footer += "<table align='left' border='0' cellspacing='0' cellpadding='0' width='22' height='22' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 0px'>";
            footer += "<tbody><tr style = 'vertical-align: top' ><td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>";
            footer += "<a href='https://fb.me/artpixxel' title='YouTube' target='_blank'>";
            footer += "<img src='cid:" + FacebookThumb.ContentId + "' alt='facebook' title='facebook' width='22' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 22px !important'>";
            footer += "</a> </td></tr></tbody></table><!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]--></div></div></td></tr></tbody></table>";
            footer += "<table style='font-family:'Montserrat',sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'><tbody><tr>";
            footer += "<td style='overflow-wrap:break-word;word-break:break-word;padding:10px 10px 20px;font-family:'Montserrat',sans-serif;' align='left'>";
            footer += "<div class='v-text-align' style='color: #333333; line-height: 180%; text-align: center; word-wrap: break-word;'>";
            footer += "<p style='font-size: 14px; line-height: 180%;'><span style='font-size: 12px; line-height: 21.6px;'>United States.</span></p>";
            footer += "<p style='font-size: 14px; line-height: 180%;'><span style='font-size: 12px; line-height: 21.6px;'><a href = 'http://www.artpixxel.com' target ='_blank'> ArtPixxel</a>&nbsp; &copy;" + DateTime.Now.Year + ". All rights reserved. </span></p>";
            footer += "</div></td></tr></tbody></table><!--[if (!mso)&(!IE)]><!--></div><!--<![endif]--></div></div><!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->";
            footer += "</div></div></div>";



            return footer;
        }


        private async Task<ValidityBoolResponse> ValidateNewPassword(SMTPModel request)
        {
            try
            {
                EmailSendStatus check = await SendNewPasswordValidationMail(request);
                return new ValidityBoolResponse
                {
                    Valid = check.Succeeded,
                    Message = check.Message

                };

            }
            catch (Exception ex)
            {

                return new ValidityBoolResponse
                {
                    Valid = false,
                    Message = ex.Message
                };
            }
        }

        private async Task<ValidityBoolResponse> ValidateConfig(SMTPModel request)
        {
            try
            {
                EmailSendStatus check = await SendConfigValidationMail(request);
                return new ValidityBoolResponse
                {
                    Valid = check.Succeeded,
                    Message = check.Message

                };

            }
            catch (Exception ex)
            {

                return new ValidityBoolResponse
                {
                    Valid = false,
                    Message = ex.Message
                };
            }
        }

        private async Task<ValidityBoolResponse> Validate(SMTPModel request)
        {
            try
            {
                EmailSendStatus check = await SendValidationMail(request);
                return new ValidityBoolResponse
                {
                    Valid = check.Succeeded,
                    Message = check.Message

                };

            }
            catch (Exception ex)
            {

                return new ValidityBoolResponse
                {
                    Valid = false,
                    Message = ex.Message
                };
            }
        }




        private async Task<List<SMTPModel>> RecentSMTPs(Pagination pagination)
        {
            try
            {
                return await _context.SMTPs.Skip(pagination.Skip).Take(pagination.PageSize).OrderBy(e => e.CreatedOn).Select(s => new SMTPModel
                {
                    Id = s.Id,
                    SenderName = s.SenderName,
                    Description = s.Description,
                    SMTPPassword = string.Empty,
                    SMTPPort = s.SMTPPort,
                    SMTPServer = s.SMTPServer,
                    SMTPState = s.SMTPState.ToString(),
                    Name = s.Name,
                    NewSMTPPassword = string.Empty,
                    SMTPUserName = s.SMTPUserName
                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<SMTPInit> Init(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);

                return new SMTPInit
                {
                    SMTPs = await RecentSMTPs(@pagination)
                  
                 
                };

               
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BaseBoolResponse> Exists(SMTPModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.SMTPs.AnyAsync(e => e.Name == @request.Name))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "SMTP with name: <i>" + @request.Name + " </i> already exists."
                    };
                }

                var check = await Validate(request);
                return new BaseBoolResponse
                {
                    Exist = !check.Valid,
                    Message = check.Message
                };
            }
            catch (Exception ex)
            {

                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseBoolResponse> Duplicate(SMTPModel request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.SMTPs.AnyAsync(e => e.Name == @request.Name && (e.Id != @request.Id)))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "SMTP with name: <i>" + @request.Name + " </i> already exists."
                    };
                }



                ValidityBoolResponse check = new();
                if(string.IsNullOrEmpty(@request.SMTPPassword) && (string.IsNullOrEmpty(@request.NewSMTPPassword)))
                {
                    check = await ValidateConfig(request);
                }

                else
                {
                    check = await ValidateNewPassword(request);
                }

             
                return new BaseBoolResponse
                {
                    Exist = !check.Valid,
                    Message = check.Message
                };
            }
            catch (Exception ex)
            {

                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = ex.Message
                };
            }
        }

        public async Task<SMTPCRUDResponse> Create(SMTPWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (!await _context.SMTPs.AnyAsync(e => e.Name == @request.SMTP.Name))
                {

                    SMTP sMTP = new()
                    {
                        Name = @request.SMTP.Name,
                        SMTPUserName = @request.SMTP.SMTPUserName,
                        SenderName = @request.SMTP.SenderName,
                        SMTPPort = @request.SMTP.SMTPPort,
                        SMTPPassword = @request.SMTP.SMTPPassword.Encrypt(_appkeys.EncryptionKey),
                        SMTPServer = @request.SMTP.SMTPServer,
                        SMTPState = (SMTPState)Enum.Parse(typeof(SMTPState), @request.SMTP.SMTPState),
                        Description = @request.SMTP.Description
                        
                    };

                    _context.SMTPs.Add(sMTP);
                    int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {
                        return new SMTPCRUDResponse
                        {
                            SMTPs = await RecentSMTPs(@request.Pagination),
                            Response = new BaseResponse
                            {
                                Message = "SMTP configuration created.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };
                    }



                    return new SMTPCRUDResponse
                    {
                        SMTPs = await RecentSMTPs(@request.Pagination),
                        Response = new BaseResponse
                        {
                            Message = "SMTP configuration could not be create. Please try again later.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Unknown Error"
                        }
                    };


                }

                return new SMTPCRUDResponse
                {
                    SMTPs = await RecentSMTPs(@request.Pagination),
                    Response = new BaseResponse
                    {
                        Message = "SMTP configuration with name: " + @request.SMTP.Name + "  already exists.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Duplicate Configuration"
                    }
                };


            }
            catch (Exception ex)
            {

                return new SMTPCRUDResponse
                {
                    SMTPs = await RecentSMTPs(@request.Pagination),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<SMTPCRUDResponse> Update(SMTPWriteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                if(await _context.SMTPs.AnyAsync(e => e.Id == request.SMTP.Id))
                {
                    SMTP sMTP = await _context.SMTPs.FindAsync(request.SMTP.Id);
                    string oldName = sMTP.Name;
                    if(sMTP != null)
                    {
                        if(!string.IsNullOrEmpty(request.SMTP.NewSMTPPassword) && (!string.IsNullOrEmpty(request.SMTP.SMTPPassword)))
                        {
                            if(@request.SMTP.SMTPPassword.Encrypt(_appkeys.EncryptionKey) != sMTP.SMTPPassword)
                            {
                                return new SMTPCRUDResponse
                                {
                                    SMTPs = await RecentSMTPs(@request.Pagination),
                                    Response = new BaseResponse
                                    {
                                        Message = "Supplied current SMTP password does not match the actual current password.",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Incorrect Password"
                                    }
                                };
                            }


                            sMTP.SMTPPassword = @request.SMTP.NewSMTPPassword.Encrypt(_appkeys.EncryptionKey);

                        }
                        sMTP.Name = @request.SMTP.Name;
                        sMTP.SMTPUserName = @request.SMTP.SMTPUserName;
                        sMTP.SenderName = @request.SMTP.SenderName;
                        sMTP.SMTPPort = @request.SMTP.SMTPPort;
                        sMTP.SMTPServer = @request.SMTP.SMTPServer;
                        sMTP.SMTPState = (SMTPState)Enum.Parse(typeof(SMTPState), @request.SMTP.SMTPState);
                        sMTP.Description = @request.SMTP.Description;

                        _context.SMTPs.Update(sMTP);
                       int saveResult = await _context.SaveChangesAsync();

                        if(saveResult > 0)
                        {
                            return new SMTPCRUDResponse
                            {
                                SMTPs = await RecentSMTPs(@request.Pagination),
                                Response = new BaseResponse
                                {
                                    Message = "SMTP configuration " + oldName + "  updated.",
                                    Result = RequestResult.Success,
                                    Succeeded = true,
                                    Title = "Successful"
                                }
                            };
                        }
                    }
                }


                return new SMTPCRUDResponse
                {
                    SMTPs = await RecentSMTPs(@request.Pagination),
                    Response = new BaseResponse
                    {
                        Message = "SMTP configuration with name: " + @request.SMTP.Name + "  does not exist. This configuration may have been deleted.",
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null SMTP Reference"
                    }
                };
            }
            catch (Exception ex)
            {

                return new SMTPCRUDResponse
                {
                    SMTPs = await RecentSMTPs(@request.Pagination),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<SMTPCRUDResponse> Delete(SMTPDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await _context.SMTPs.AnyAsync(e => e.Id == @request.Id))
                {
                    SMTP sMTP = await _context.SMTPs.FindAsync(@request.Id);
                    if(sMTP != null)
                    {
                        if(await _context.SMTPs.AnyAsync(e => e.Id != @request.Id))
                        {
                            SMTP nextSMTP = await _context.SMTPs.Where(e => e.Id != @request.Id).FirstOrDefaultAsync();
                            if(nextSMTP != null)
                            {
                                nextSMTP.SMTPState = SMTPState.Active;
                                _context.SMTPs.Update(nextSMTP);
                                await _context.SaveChangesAsync();
                            }
                        }


                        sMTP.SMTPState = SMTPState.Inactive;
                        _context.SMTPs.Remove(sMTP);
                        await _context.SaveChangesAsync();



                        return new SMTPCRUDResponse
                        {
                            SMTPs = await RecentSMTPs(@request.Pagination),
                            Response = new BaseResponse
                            {
                                Message = sMTP.Name +" removed.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            }
                        };




                    }
                }


                return new SMTPCRUDResponse
                {
                    SMTPs = await RecentSMTPs(@request.Pagination),
                    Response = new BaseResponse
                    {
                        Message = "SMTP configuration could not be resolved. This SMTP may have been deleted." ,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = "Null SMTP Reference"
                    }
                };

            }
            catch (Exception ex)
            {

                return new SMTPCRUDResponse
                {
                    SMTPs = await RecentSMTPs(@request.Pagination),
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    }
                };
            }
        }

        public async Task<EmailSendStatus> SendWelcomeMail(AccountModel request)
        {
            try
            {
                if (await _context.SMTPs.AnyAsync(e => e.SMTPState == SMTPState.Active))
                {
                    SMTP sMTP = await _context.SMTPs.Where(e => e.SMTPState == SMTPState.Active).FirstOrDefaultAsync();
                    if (sMTP != null)
                    {
                      

                            if (!string.IsNullOrEmpty(request.EmailAddress))
                            {



                            string emailBody = "<p>Hi " + request.FirstName+", we are so thrilled you decided to join the ARTPIXXEL family! Hats off on making new decision." +
                           " You are now officially in the loop to hear all about our awesome products, new releases and maybe a deal or two. Here are your credentials to login and access your customer account: " +
                           "<br/> <b>Username: </b> "+request.Username+" <br/> <b>Password:</b> "+request.Password+"</p>";
                          
                            emailBody += "<p>We are not all about wall arts! Please check out our social media pages and discover our quirky little world.</p>";
                            emailBody += "<p>Welcome</p><p>Kind regards.</p>";
                            emailBody += "<br/><br/><br/><br/>";





                            EmailTemplate emailTemplate = new EmailTemplate
                                {
                                    ActionButton = true,
                                    URL = _currentUserService.GetBaseURl(),
                                    SubTitle = "Welcome to Artpixxel",
                                    HasHeroImage = true,
                                    Body = emailBody,
                                    EmailType = EmailType.Welcome,
                                    Title = "Welcome to Artpixxel",
                            };

                                EmailModel model = new EmailModel
                                {
                                    Body = emailTemplate,
                                    Emails = new List<string> { request.EmailAddress },
                                    SMTP = sMTP,
                                    Subject = "Welcome to Artpixxel",

                                };




                                MimeMessage message = new MimeMessage();
                                BodyBuilder builder = new BodyBuilder();
                                InternetAddressList EmailList = new InternetAddressList();

                                foreach (string email in model.Emails)
                                {

                                    EmailList.Add(MailboxAddress.Parse(email));
                                }

                                builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                                message.To.AddRange(EmailList);
                                message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                                message.Subject = model.Subject;
                                message.Date = DateTime.Now;
                                message.Body = builder.ToMessageBody();

                                return await EmailSender(message, model.SMTP);


                            }


                            return new EmailSendStatus
                            {
                                Message = "Your email wasn't found. Test email couldn't be sent.",
                                Succeeded = false
                            };


                        
                    }

                }

                return new EmailSendStatus
                {
                    Message = "SMTP user not found.",
                    Succeeded = false
                };



            }
            catch (Exception ex)
            {
                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false

                };
               
            }
        }


        

        private async Task<string> OrderDetails(CheckoutNotification checkout, BodyBuilder builder)
        {
            string content = "<table  role='presentation'cellpadding='0' cellspacing ='0' width='100%' border='0'>";
            content += "<tr><td align='left' valign='top' style='padding: 5px  5px 5px 5px;'>";
            content += "<p style='font-weight: bold; padding: 0; margin: 0;'>[DELIVERY]</p></td></tr>";
            content += "<tr><td align='left' valign='top' style='padding: 3px  5px 2px 5px;'>";
            content += "Delivery between " + checkout.Order.SpeculativeDeliveryDate.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true) + " and " + checkout.Order.DeliveryDate.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true) + "</td></tr></table>";
            content += "<table style='width:100%;border:none;border-spacing:0; font-size:14px; margin: 20px 0 20px 0;' role = 'presentation' cellpadding = '0' cellspacing = '5' width = '100%' border = '0'>";
            content += "<tr><td align='left' valign='top' style='padding: 10px  5px 10px 5px;'><p style = 'font-weight: bold; padding: 0; margin: 0'>[ORDER DETAILS] </p></td></tr></table>";



            content += "<!--[if gte mso 9]><table width='600' cellpadding='0' cellspacing='0'><tr><td><![endif]-->";
            content += "<table class='container600' cellpadding='0' cellspacing='0' border='0' width='100%' style='width:calc(100%);max-width:calc(600px);margin: 0 auto;'>";
            content += "<tr><td width = '100%' style='text-align: left;'>";
            content += "<table width = '100%' cellpadding='0' cellspacing='0' border='0' style='min-width:100%;'>";
            content += "<tr><td style = 'background-color:#f3f4f6;padding:20px;'>";
            content += "<table width='100%' cellpadding='0' cellspacing='0' border='0'> <tr><td width = '100%' valign='top' style='min-width: 100%;'>";
            MimeEntity itemImageThumb;
            int counter = 1;
            foreach (var item in checkout.Order.Items)
            {
                FileContentInfo itemImageContent = item.Category == OrderItemCategory.UploadedImage ? await item.PreviewImageURL.FileContent() : await item.ImageURL.FileContent();
                itemImageThumb = builder.LinkedResources.Add(itemImageContent.FileName, itemImageContent.Bytes, itemImageContent.ContentType);
                itemImageThumb.ContentId = (Generators.Generate(new GenerationOptions(useSpecialCharacters: false, useNumbers: true, length: 20))).ToUpper();

                content += "<!--[if gte mso 9]><table width = '100%' cellpadding='0' cellspacing='0'><tr><![endif]-->";
                content += "<!--[if gte mso 9]><td width = '49%' valign='top'><![endif]-->";
                content += "<table class='ol col49 reorder' width='100%' align='left' style='width:calc(49%);' cellpadding='0' cellspacing='0' border='0'>";
                content += "<tr><td width = '100%' valign='top' style='color:#000000; padding: 5px;'>";
                content += "<table width='100%' cellpadding='0' cellspacing='0' border='0' style='min-width:100%;'>";
                content += "<tr><td width='100%' style='min-width:100%;background-color:#ffffff; padding: 10px;'>";
                content += "<table cellpadding='0' cellspacing='0' border='0' width='100%'>";
                content += "<tr><td style = 'padding-top:10px;padding-bottom:10px;'>";
                content += "<h6 style='font-size:14px;line-height:1; margin: 0; padding: 0; width:210px;white-space: nowrap;overflow: hidden;text-overflow: ellipsis; '>" + (string.IsNullOrEmpty(item.Heading) ? "Heading" : item.Heading) + "</h6>";
                content += "</td></tr></table><img alt = " + item.Description + " src='cid:" + itemImageThumb.ContentId + "' width='210' style='display: block; margin-bottom: 10px;'/>";
                content += "<table cellpadding = '0' cellspacing='0' border='0' width='100%'>";
                content += "<tr><td style = 'padding: 15px 5px 5px 5px; background-color:#f9fafc;'>";
                content += "<p style='font-size:13px;line-height:1; margin: 0; padding: 0 0 0 5px;'>Category: " + item.Category.ToString() + "</p></td></tr>";
                content += "<tr><td style = 'padding: 5px; background-color:#f9fafc;'><p style= 'font-size:13px;line-height:1; margin: 0;  padding: 0 0 0 5px'> Price: $";
                content += decimal.Round(item.TotalAmount, 2, MidpointRounding.AwayFromZero).ToString("#.##") + "</p></td></tr>";
                content += "<tr><td style = 'padding: 5px; background-color:#f9fafc;'><p style= 'font-size:13px;line-height:1; margin: 0;  padding: 0 0 0 5px'> Quantity: " + item.Quantity + "</p></td></tr>";
                content += "<tr><td style = 'padding: 5px; background-color:#f9fafc;'><p style= 'font-size:13px;line-height:1; margin: 0;  padding: 0 0 0 5px'> Size: ";
                content += (string.IsNullOrEmpty(item.SizeName) ? "Default" : item.SizeName) + "</p></td></tr>";
                content += "<tr><td style = 'padding:5px; background-color:#f9fafc;'><p style= 'font-size:13px;line-height:1; margin: 0;  padding: 0 0 0 5px'> Frame: " + item.FrameClass + "</p></td></tr>";
                content += "</table></td></tr></table></td></tr></table>";
                content += "<!--[if gte mso 9]></td><![endif]-->";
                if ((counter % 2) != 0)
                {
                    content += "<!--[if gte mso 9]><td width='2%' valign ='top'><![endif]-->";
                    content += "<table class='ol col2 ghost-column' width='100%' align='left' style='width: calc(2%);' cellpadding='0' cellspacing='0' border='0'>";
                    content += "<tr><td width = '100%' valign='top' style='line-height: 20px;'>";
                    content += "&nbsp;</td></tr></table>";
                    content += "<!--[if gte mso 9]></td><![endif]-->";
                }
                content += "<!--[if gte mso 9]></td></tr></table><![endif]-->";

                counter++;

            }
            content += "</td></tr></table>";
            content += "</td></tr></table><!--[if gte mso 9]></td></tr></table><![endif]--> </td></tr></table>";
            content += "<table class='ol col100' width='100%' align='left' style='width: calc(100%); font-size:12px;' cellpadding='0' cellspacing='0' border='0'>";
            content += "<tr><td align='left' valign='center'  style='padding: 20px  5px 20px 5px;border-bottom: 1px solid #eeeeee;background-color: #ffffff;'>SUBTOTAL</td>";
            content += "<td align = 'left' valign='center' style='padding: 20px  5px 20px 5px;border-bottom: 1px solid #eeeeee;background-color: #ffffff;'>";
            content += "$" + decimal.Round(checkout.Order.SubTotal, 2, MidpointRounding.AwayFromZero).ToString("#.##") + "</td></tr>";
            content += "<tr><td align = 'left' valign='center' style='border-bottom: 1px solid #eeeeee;padding: 20px  5px 20px 5px;background-color: #ffffff;'> VAT</td>";
            content += "<td align='left' valign='center' style='border-bottom: 1px solid #eeeeee;padding: 20px  5px 20px 5px;background-color: #ffffff;'>";
            content +="$"+decimal.Round(checkout.Order.VAT, 2, MidpointRounding.AwayFromZero).ToString("#.##") + "</td>";
            content += "<tr><td align ='left' valign='center' style='padding: 20px  5px 20px 5px;background-color: #ffffff;'>DELIVERY</td>";
            content += "<td align = 'left' valign='center' style='padding: 20px  5px 20px 5px;background-color: #ffffff;'>";
            content += "$" + decimal.Round(checkout.Order.DeliveryFee, 2, MidpointRounding.AwayFromZero).ToString("#.##") + "</td></tr>";
            content += "<tr><td align='left'  valign='center' style='font-weight: bold;padding: 20px 5px 20px 5px;background-color: #e7e9f0;'>Total</td>";
            content += "<td align = 'left' valign='center' style='font-weight: bold;padding: 20px 5px 20px 5px;background-color: #e7e9f0;''>";
            content += "$"+decimal.Round(checkout.Order.GrandTotal, 2, MidpointRounding.AwayFromZero).ToString("#.##") + "</td></tr></table>";


            return content;

        }



        public async Task<EmailSendStatus> SendAdminOrderNotification(CheckoutNotification checkoutNotification)
        {
            try
            {
                if (await _context.SMTPs.AnyAsync(e => e.SMTPState == SMTPState.Active))
                {
                    SMTP sMTP = await _context.SMTPs.Where(e => e.SMTPState == SMTPState.Active).FirstOrDefaultAsync();
                    if (sMTP != null)
                    {


                        List<string> adminEmails = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Email).ToListAsync();   //new List<string> { DefaultEmails.SuperAdminEmail };  
                        if(!adminEmails.Any())
                        {
                            adminEmails = new List<string> { DefaultEmails.AdminEmail };
                        }

                        if (adminEmails.Any())
                        {
                            // Invoice number and order date

                            string content = "<table style = 'font-family:'Montserrat',sans-serif; font-size:12px;' role = 'presentation' cellpadding = '0' cellspacing = '0' width = '100%' border = '0'>";
                            content += "<tr><td align = 'left' valign='top' style ='padding: 5px  5px 5px 5px;'>ORDER  ID: " + checkoutNotification.Order.InvoiceNumber + "</td></tr>";
                            content += "<tr><td align = 'left' valign = 'top' style = 'padding: 0px  5px 5px 5px;'>ORDER  DATE: " + checkoutNotification.Order.CreatedOn.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true) + "</td></tr></table>";
                            // Customer Details
                            content += "<table style = 'font-family:'Montserrat',sans-serif; font-size:12px; margin: 20px 0 ;' role = 'presentation' cellpadding = '0' cellspacing = '0' width = '100%' border = '0'>";
                            content += "<tr><td><table style = 'font-family:'Montserrat',sans-serif; font-size:12px ;' role = 'presentation' cellpadding = '0' cellspacing = '0' width = '100%' border = '0'>";
                            content += "<tr><td align = 'left' valign = 'top' style = 'padding: 5px  5px 5px 5px;'>";
                            content += "<p style = 'font-weight: bold ;' >[BILLLING] </p>";
                            content += "</td></tr><tr><td align = 'left' valign = 'top' style = 'padding: 0px  5px 0px 5px;' >Name: " + checkoutNotification.FullName + "</td></tr>";
                            content += "<tr><td align = 'left' valign = 'top' style = 'padding: 0px  5px 0px 5px;'>Phone Number: " +( string.IsNullOrEmpty(checkoutNotification.AdditionalPhoneNumber) ? checkoutNotification.MobilePhoneNumber: checkoutNotification.MobilePhoneNumber +","+ checkoutNotification.AdditionalPhoneNumber)  + "</td></tr>";
                            content += "<tr><td align = 'left' valign = 'top' style='padding: 0px  5px 10px 5px;'>Email: "+ checkoutNotification.EmailAddress + "</td></tr>";
                            content += "</table></td><td><table style = 'font-family:'Montserrat',sans-serif; font-size:12px ;' role = 'presentation' cellpadding = '0' cellspacing = '0' width = '100%' border = '0' >";
                            content += "<tr><td align = 'left' valign = 'top' style = 'padding: 5px  5px 5px 5px;'>";
                            content += "<p style='font-weight: bold ;'>[SHIPPING] </p></td></tr>";
                            content += "<tr><td align ='left' valign = 'top' style ='padding: 0px  5px 0px 5px;'>Country: " + checkoutNotification.Country + "</td></tr>";
                            content += "<tr><td align = 'left' valign='top' style ='padding: 0px  5px 0px 5px;'>State: " + checkoutNotification.State + "</td></tr>";
                            content += "<tr><td align = 'left' valign ='top' style ='padding: 0px  5px 0px 5px;'>City: " + checkoutNotification.City + "</td></tr>";
                            content += "<tr><td align = 'left' valign = 'top' style = 'padding: 0px  5px 0px 5px;'>Shipping Address: " + checkoutNotification.ShippingAddress + "</td></tr>";
                            content += "<tr><td align = 'left' valign = 'top' style = 'padding: 0px  5px 0px 5px;'>Zip Code: " + checkoutNotification.ZipCode + "</td></tr>";
                            content += "</table></td></tr></table>";







                           EmailTemplate emailTemplate = new EmailTemplate
                            {
                                ActionButton = true,
                                URL = _currentUserService.GetBaseURl(),
                                SubTitle = "",
                                HasHeroImage = false,
                                Body = content,
                                EmailType = EmailType.Order,
                                Title = "New Order",
                            };

                            EmailModel model = new EmailModel
                            {
                                Body = emailTemplate,
                                Emails = adminEmails.Any() ? adminEmails : new List<string> { DefaultEmails.AdminEmail},
                                SMTP = sMTP,
                                Subject = "New Order",

                            };




                            MimeMessage message = new MimeMessage();
                            BodyBuilder builder = new BodyBuilder();
                            InternetAddressList EmailList = new InternetAddressList();

                            foreach (string email in model.Emails)
                            {

                                EmailList.Add(MailboxAddress.Parse(email));
                            }

                            builder.HtmlBody = await ComposeHTMLBody(builder, model.Body, checkoutNotification);
                            message.To.AddRange(EmailList);
                            message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                            message.Subject = model.Subject;
                            message.Date = DateTime.Now;
                            message.Body = builder.ToMessageBody();

                            return await EmailSender(message, model.SMTP);


                        }


                        return new EmailSendStatus
                        {
                            Message = "Your email wasn't found. Test email couldn't be sent.",
                            Succeeded = false
                        };



                    }

                }

                return new EmailSendStatus
                {
                    Message = "SMTP user not found.",
                    Succeeded = false
                };



            }
            catch (Exception ex)
            {
                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false

                };

            }
        }

        public async Task<EmailSendStatus> SendCustomerOrderNotification(CheckoutNotification checkoutNotification)
        {
            try
            {
                if (await _context.SMTPs.AnyAsync(e => e.SMTPState == SMTPState.Active))
                {
                    SMTP sMTP = await _context.SMTPs.Where(e => e.SMTPState == SMTPState.Active).FirstOrDefaultAsync();
                    if (sMTP != null)
                    {

                      


                        if (!string.IsNullOrEmpty(checkoutNotification.EmailAddress))
                        {



                            string content = "<table style = 'font-family:'Montserrat',sans-serif; font-size:12px;' role='presentation' cellpadding = '0' cellspacing = '0' width='100%' border ='0'>";
                            content += "<tr><td align = 'left' valign='top' style ='padding: 5px  5px 5px 5px;'> We've received your order and will contact you as as soon as your package is shipped. You can find your purchase information below. </td></tr></table>";
                            // Invoice number and order date

                            content += "<table style='font-family:'Montserrat',sans-serif; font-size:12px;' role='presentation' cellpadding ='0' cellspacing='0' width='100%' border = '0'>";
                            content += "<tr><td align = 'left' valign='top' style ='padding: 5px  5px 5px 5px;'>ORDER  ID: " + checkoutNotification.Order.InvoiceNumber + "</td></tr>";
                            content += "<tr><td align ='left' valign ='top' style='padding: 0px  5px 5px 5px;'>ORDER  DATE: " + checkoutNotification.Order.CreatedOn.ToString("dddd, dnn MMMM yyyy", useExtendedSpecifiers: true) + "</td></tr></table>";
                           






                            EmailTemplate emailTemplate = new EmailTemplate
                            {
                                ActionButton = true,
                                URL = _currentUserService.GetBaseURl(),
                                SubTitle = checkoutNotification.FullName+",  thank you for your order!",
                                HasHeroImage = false,
                                Body = content,
                                EmailType = EmailType.Order,
                                Title = "Order Confirmation",
                            };

                            EmailModel model = new EmailModel
                            {
                                Body = emailTemplate,
                                Emails = new List<string> { checkoutNotification.EmailAddress },
                                SMTP = sMTP,
                                Subject = "Order Confirmation",

                            };




                            MimeMessage message = new MimeMessage();
                            BodyBuilder builder = new BodyBuilder();
                            InternetAddressList EmailList = new InternetAddressList();

                            foreach (string email in model.Emails)
                            {

                                EmailList.Add(MailboxAddress.Parse(email));
                            }

                            builder.HtmlBody = await ComposeHTMLBody(builder, model.Body, checkoutNotification);
                            message.To.AddRange(EmailList);
                            message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                            message.Subject = model.Subject;
                            message.Date = DateTime.Now;
                            message.Body = builder.ToMessageBody();

                            return await EmailSender(message, model.SMTP);


                        }


                        return new EmailSendStatus
                        {
                            Message = "Your email wasn't found. Test email couldn't be sent.",
                            Succeeded = false
                        };



                    }

                }

                return new EmailSendStatus
                {
                    Message = "SMTP user not found.",
                    Succeeded = false
                };



            }
            catch (Exception ex)
            {
                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false

                };

            }
        }

        public async Task<EmailSendStatus> SendOrderStatusNotification(OrderStatusMail statusMail)
        {
            try
            {
                if (await _context.SMTPs.AnyAsync(e => e.SMTPState == SMTPState.Active))
                {
                    SMTP sMTP = await _context.SMTPs.Where(e => e.SMTPState == SMTPState.Active).FirstOrDefaultAsync();
                    if (sMTP != null)
                    {


                        if (!string.IsNullOrEmpty(statusMail.EmailAddress))
                        {



                            string emailBody = "<p>" + statusMail.Message +  "</p>";
                            emailBody += "<br/><br/><br/><br/>";





                            EmailTemplate emailTemplate = new EmailTemplate
                            {
                                ActionButton = true,
                                URL = _currentUserService.GetBaseURl(),
                                SubTitle = "Order ID: "+ statusMail.OrderId,
                                HasHeroImage = true,
                                Body = emailBody,
                                EmailType = EmailType.Notification,
                                Title = "Order Status Notification",
                            };

                            EmailModel model = new EmailModel
                            {
                                Body = emailTemplate,
                                Emails = new List<string> { statusMail.EmailAddress },
                                SMTP = sMTP,
                                Subject = "Order Status Notification",

                            };




                            MimeMessage message = new MimeMessage();
                            BodyBuilder builder = new BodyBuilder();
                            InternetAddressList EmailList = new InternetAddressList();

                            foreach (string email in model.Emails)
                            {

                                EmailList.Add(MailboxAddress.Parse(email));
                            }

                            builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                            message.To.AddRange(EmailList);
                            message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                            message.Subject = model.Subject;
                            message.Date = DateTime.Now;
                            message.Body = builder.ToMessageBody();

                            return await EmailSender(message, model.SMTP);


                        }


                        return new EmailSendStatus
                        {
                            Message = "Recipient email is null",
                            Succeeded = false
                        };



                    }

                }

                return new EmailSendStatus
                {
                    Message = "No SMTP configuration not found.",
                    Succeeded = false
                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmailSendStatus> SendContactNotification(ContactRequest contact)
        {
            try
            {
                if (await _context.SMTPs.AnyAsync(e => e.SMTPState == SMTPState.Active))
                {
                    SMTP sMTP = await _context.SMTPs.Where(e => e.SMTPState == SMTPState.Active).FirstOrDefaultAsync();
                    if (sMTP != null)
                    {
                       // List<string> adminEmails = new();
                        List<string> adminEmails = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Email).ToListAsync();
                        if (!adminEmails.Any())
                        {
                            adminEmails = new List<string> { DefaultEmails.AdminEmail };
                        }

                        string  content = "<table style='font-family:'Montserrat',sans-serif; font-size:12px;' role='presentation' cellpadding ='0' cellspacing='0' width='100%' border = '0'>";
                        content += "<tr><td align = 'left' valign='top' style ='padding: 0px  5px 0px 5px;'>Name: " + contact.Name + "</td></tr>";
                        content += "<tr><td align = 'left' valign='top' style ='padding: 0px  5px 0px 5px;'>Phone Number: " + contact.PhoneNumber + "</td></tr>";
                        content += "<tr><td align ='left' valign ='top' style='padding: 0px  5px 30px 5px;'>Email Address: " + contact.EmailAddress + "</td></tr></table>";

                        content += "<table style = 'font-family:'Montserrat',sans-serif; font-size:12px;' role='presentation' cellpadding = '0' cellspacing = '0' width='100%' border ='0'>";
                        content += "<tr><td align = 'left' valign='top' style ='padding: 15px  15px 15px 15px; background-color:#eeeff5; border-radius:10px'> " + contact.Message +" </td></tr></table>";

                        EmailTemplate emailTemplate = new()
                        {
                            ActionButton = false,
                            URL = _currentUserService.GetBaseURl(),
                            SubTitle = contact.Subject,
                            HasHeroImage = false,
                            Body = content,
                            EmailType = EmailType.Notification,
                            Title = "New Contact Message",
                        };

                        EmailModel model = new EmailModel
                        {
                            Body = emailTemplate,
                            Emails = adminEmails.Any() ? adminEmails : new List<string> { DefaultEmails.AdminEmail },
                            SMTP = sMTP,
                            Subject = "New Contact Message",

                        };


                        MimeMessage message = new MimeMessage();
                        BodyBuilder builder = new BodyBuilder();
                        InternetAddressList EmailList = new InternetAddressList();

                        foreach (string email in model.Emails)
                        {

                            EmailList.Add(MailboxAddress.Parse(email));
                        }

                        builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                        message.To.AddRange(EmailList);
                        message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                        message.Subject = model.Subject;
                        message.Date = DateTime.Now;
                        message.Body = builder.ToMessageBody();

                       EmailSendStatus  res = await EmailSender(message, model.SMTP);

                        return res;

                    }

                }


                return new EmailSendStatus
                {
                    Message = "No SMTP configuration found.",
                    Succeeded = false
                };
            }
            catch (Exception ex)
            {

                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false
                };
            }
        }

        public async Task<EmailSendStatus> SendPasswordResetToken(TokentNotificationModel request)
        {
            try
            {
                if (await _context.SMTPs.AnyAsync(e => e.SMTPState == SMTPState.Active))
                {
                    SMTP sMTP = await _context.SMTPs.Where(e => e.SMTPState == SMTPState.Active).FirstOrDefaultAsync();
                    if (sMTP != null)
                    {


                        if (!string.IsNullOrEmpty(request.EmailAddress))
                        {



                             byte[] _tokenBytes = Encoding.UTF8.GetBytes(request.Token);
                             string _token = Convert.ToBase64String(_tokenBytes);

                            string emailBody = "<p>Hi " + request.FullName + ", a password reset token has been generated for you. This token expires in 30mins and becomes invalid for the purpose it was created." +
                           "</p>";

                            emailBody += "<p>Please apply the reset token <a href='"+ _currentUserService.GetBaseURl() + "/account/password-reset/t/" + _token+"'> here</a> to change your password.</p>";
                            emailBody += "Kind regards.</p>";
                            emailBody += "<br/><br/><br/><br/>";





                            EmailTemplate emailTemplate = new EmailTemplate
                            {
                                ActionButton = true,
                                URL = _currentUserService.GetBaseURl(),
                                SubTitle = "Password Reset Token",
                                HasHeroImage = true,
                                Body = emailBody,
                                EmailType = EmailType.PasswordChange,
                                Title = "Password Reset Token",
                            };

                            EmailModel model = new EmailModel
                            {
                                Body = emailTemplate,
                                Emails = new List<string> { request.EmailAddress },
                                SMTP = sMTP,
                                Subject = "Password Reset Token",

                            };




                            MimeMessage message = new MimeMessage();
                            BodyBuilder builder = new BodyBuilder();
                            InternetAddressList EmailList = new InternetAddressList();

                            foreach (string email in model.Emails)
                            {

                                EmailList.Add(MailboxAddress.Parse(email));
                            }

                            builder.HtmlBody = await ComposeHTMLBody(builder, model.Body);
                            message.To.AddRange(EmailList);
                            message.From.Add(new MailboxAddress(model.SMTP.SenderName, model.SMTP.SMTPUserName));
                            message.Subject = model.Subject;
                            message.Date = DateTime.Now;
                            message.Body = builder.ToMessageBody();

                            return await EmailSender(message, model.SMTP);


                        }


                        return new EmailSendStatus
                        {
                            Message = "Your email wasn't found. Test email couldn't be sent.",
                            Succeeded = false
                        };



                    }

                }

                return new EmailSendStatus
                {
                    Message = "SMTP user not found.",
                    Succeeded = false
                };


            }
            catch (Exception ex)
            {

                return new EmailSendStatus
                {
                    Message = ex.Message,
                    Succeeded = false
                };
            }
        }
    }
}
