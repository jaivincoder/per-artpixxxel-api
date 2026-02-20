using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Contacts;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo;
using api.artpixxel.service.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Contacts
{
    public class ContactService : IContactService
    {

        private readonly ArtPixxelContext _context;
        private readonly IIPAddressService _iPAddressService;
        private readonly ISMTPService _SMTPService;
        public ContactService(ArtPixxelContext context, IIPAddressService iPAddressService, ISMTPService sMTPService)
        {
            _context = context;
            _iPAddressService = iPAddressService;
            _SMTPService = sMTPService;
        }

        public async Task<BaseResponse> Create(ContactRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(await Spam(request))
                {
                    return new BaseResponse
                    {
                        Message = "We have received your message. A member of our team will contact you shortly. Thank you",
                        Result = RequestResult.Warn,
                        Succeeded = false,
                        Title = "Duplicate Request"
                    };
                }

                else
                {
                    Contact contact = new()
                    {
                        EmailAddress = @request.EmailAddress,
                        Name= @request.Name,
                        PhoneNumber= @request.PhoneNumber,
                        Subject= @request.Subject,
                        Message= @request.Message,
                        IPAddress = _iPAddressService.IPAddress()

                    };

                    _context.Add(contact);
                   int res = await _context.SaveChangesAsync();

                    if(res > 0) 
                    
                    {
                        // also send email

                        await _SMTPService.SendContactNotification(request);
                        return new BaseResponse
                        {
                            Message = "We have received your message. A member of our team will contact you shortly. Thank you",
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = "Successful"
                        };


                    }


                }





                return new BaseResponse
                {
                    Message = "An error occurred. Please try again later.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Network Error"
                };

            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message= ex.Message,
                    Result = RequestResult.Error,
                    Succeeded= false,
                    Title= ex.Source
                };
            }
        }

        public async Task<BaseBoolResponse> Multiple(MultipleContactRequest request)
        {
            try
            {
                if(await _context.Contacts.AnyAsync(a => (a.EmailAddress == request.EmailAddress || (a.PhoneNumber == request.PhoneNumber))
           && (a.IPAddress == _iPAddressService.IPAddress()) && (a.CreatedOn > DateTime.Now.AddHours(-9))))
                {
                     return new BaseBoolResponse { Exist= true, Message = "<i>Duplicate Request: </i>The message you earlier sent has been received. A member of our team will contact you shortly. Thank you" };
                }


                return new BaseBoolResponse { Exist = false, Message = string.Empty };
            }
            catch (Exception ex)
            {

                return new BaseBoolResponse { Message = ex.Message, Exist= true };
            }
        }

        private async Task<bool> Spam(ContactRequest request)
        {

            return await _context.Contacts.AnyAsync(a => (a.EmailAddress == request.EmailAddress || (a.PhoneNumber == request.PhoneNumber))
           && (a.IPAddress == _iPAddressService.IPAddress()) && (a.CreatedOn > DateTime.Now.AddHours(-9))
           );
        }

        public async Task<ContactListInit> List(Pagination pagination)
        {
            var list = await _context.Contacts
                .OrderByDescending(c => c.CreatedOn)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .Select(c => new ContactListResponse
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    EmailAddress = c.EmailAddress ?? string.Empty,
                    PhoneNumber = c.PhoneNumber ?? string.Empty,
                    Subject = c.Subject ?? string.Empty,
                    Message = c.Message ?? string.Empty,
                    CreatedOn = c.CreatedOn.ToString(DefaultDateFormat.ddMMyyyyhhmmsstt)
                })
                .ToListAsync();

            var total = await _context.Contacts.CountAsync();

            return new ContactListInit
            {
                ContactList = list,
                TotalCount = total
            };
        }
    }
}
