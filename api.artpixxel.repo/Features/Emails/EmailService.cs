

using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using static api.artpixxel.repo.Utils.Mails.EmailFilterUtils;
using LinqKit;

namespace api.artpixxel.repo.Features.Emails
{
    public class EmailService : IEmailService
    {
        private readonly ArtPixxelContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        public EmailService(ArtPixxelContext context, ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<BaseResponse> AddToEmailList(NewsletterRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if(!await _context.EmailLists.AnyAsync(e => e.EmailAddress == @request.EmailAddress))
                {
                    EmailList emailList = new()
                    {
                        EmailAddress = @request.EmailAddress
                    };

                    _context.EmailLists.Add(emailList);
                    int saveResult = await _context.SaveChangesAsync();

                    if(saveResult > 0)
                    {
                        return new BaseResponse
                        {
                            Title = "Successful",
                            Message = "Newsletter subscription successful.",
                            Result = RequestResult.Success,
                            Succeeded = true
                        };
                    }
                }



                return new BaseResponse
                {
                    Title = "Duplicate Signup",
                    Message = "You have already subscribed to our newsletter. Thank you",
                    Succeeded = true,
                    Result = RequestResult.Info
                };

            }
            catch (Exception ex) 
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Success,
                    Succeeded = false,
                    Title = ex.Source
                };
            }
        }

        public async Task AddToEmailList(string emailAddress)
        {
            try
            {
                if (!await _context.EmailLists.AnyAsync(e => e.EmailAddress == emailAddress))
                {
                    EmailList emailList = new()
                    {
                        EmailAddress = emailAddress
                    };

                    _context.EmailLists.Add(emailList);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddToEmailList(EmailListRequest request)
        {
            try
            {
                if (!await _context.EmailLists.AnyAsync(e => e.EmailAddress == request.EmailAddress))
                {
                    EmailList emailList = new()
                    {
                        EmailAddress = request.EmailAddress,
                        FirstName = request.FirstName,
                        LastName = request.LastName
                    };

                    _context.EmailLists.Add(emailList);
                    await _context.SaveChangesAsync();

                  
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmailListCRUDResponse> Delete(EmailListDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                EmailList emailList = await _context.EmailLists.FindAsync(request.Id);
                if(emailList != null)
                {
                    _context.EmailLists.Remove(emailList);
                    await _context.SaveChangesAsync();


                    return new EmailListCRUDResponse
                    {
                        Response = new BaseResponse
                        {
                            Message = "Email deleted.",
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = "Successful"
                        },
                        Data = await MailList(request.Filter)
                    };


                }

                return new EmailListCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "Reference to email couldn't be resolved. This email may have been deleted.",
                        Result = RequestResult.Warn,
                        Succeeded = false,
                        Title = "Null Email Reference"
                    },
                    Data = await MailList(request.Filter)
                };




            }
            catch (Exception ex)
            {

                return new EmailListCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    },
                    Data = await MailList(request.Filter)
                };
            }
        }

        public async Task<List<EmailListResponse>> Export(EmailListFilterData filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                ExpressionStarter<EmailList> pred = ApplyFilter(@filter);


                return emptyFilter ?
                    await _context.EmailLists.OrderByDescending(e => e.CreatedOn)
                    .Select(e => new EmailListResponse()
                    {
                        Id = e.Id,
                        EmailAddress = e.EmailAddress,
                        FirstName = string.IsNullOrEmpty(e.FirstName) ? string.Empty : e.FirstName,
                        LastName = string.IsNullOrEmpty(e.LastName) ? string.Empty : e.LastName,
                        SignupDate = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy)


                    }).ToListAsync()

                    :

                     await _context.EmailLists.OrderByDescending(e => e.CreatedOn)
                     .Where(pred)
                    .Select(e => new EmailListResponse()
                    {
                        Id = e.Id,
                        EmailAddress = e.EmailAddress,
                        FirstName = string.IsNullOrEmpty(e.FirstName) ? string.Empty : e.FirstName,
                        LastName = string.IsNullOrEmpty(e.LastName) ? string.Empty : e.LastName,
                        SignupDate = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy)


                    }).ToListAsync()

                    ;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmailListInit> Init(Pagination pagination)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@pagination", pagination);


                return new EmailListInit
                {
                    EmailList = await TopMost(pagination),
                    TotalEmail = decimal.Round(await _context.EmailLists.CountAsync(), 0, MidpointRounding.AwayFromZero)
                };

                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmailListInit> MailList(EmailListFilterData filter)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@filter", filter);

                bool emptyFilter = EmptyFilter(filter);
                ExpressionStarter<EmailList> pred = ApplyFilter(@filter);

                List<EmailListResponse> list = emptyFilter ?
                    await _context.EmailLists.OrderByDescending(e => e.CreatedOn)
                    .Skip(filter.Skip)
                    .Take(filter.PageSize)
                    .Select(e => new EmailListResponse()
                    {
                        Id = e.Id,
                        EmailAddress = e.EmailAddress,
                        FirstName = string.IsNullOrEmpty(e.FirstName) ? string.Empty : e.FirstName,
                        LastName = string.IsNullOrEmpty(e.LastName) ? string.Empty : e.LastName,
                        SignupDate = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy)


                    }).ToListAsync()
                    
                    :

                     await _context.EmailLists.OrderByDescending(e => e.CreatedOn)
                     .Where(pred)
                    .Skip(filter.Skip)
                    .Take(filter.PageSize)
                    .Select(e => new EmailListResponse()
                    {
                        Id = e.Id,
                        EmailAddress = e.EmailAddress,
                        FirstName = string.IsNullOrEmpty(e.FirstName) ? string.Empty : e.FirstName,
                        LastName = string.IsNullOrEmpty(e.LastName) ? string.Empty : e.LastName,
                        SignupDate = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy)


                    }).ToListAsync()

                    ;

                list = ApplySort(list, filter);


                return new EmailListInit
                {
                    EmailList = list,
                    TotalEmail = decimal.Round(emptyFilter ? await _context.EmailLists.CountAsync() : await _context.EmailLists.Where(pred).CountAsync(), 0, MidpointRounding.AwayFromZero)
                };




            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmailListCRUDResponse> MultiDelete(EmailListMultiDeleteRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                List<EmailList> emailLists = await _context.EmailLists.Where(e => request.Ids.Contains(e.Id)).ToListAsync();
                if (emailLists.Any())
                {
                    _context.EmailLists.RemoveRange(emailLists);
                    await _context.SaveChangesAsync();


                    return new EmailListCRUDResponse
                    {
                        Response = new BaseResponse
                        {
                            Message =  "Selected email "+(emailLists.Count == 1 ? "list": "lists") + " deleted.",
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = "Successful"
                        },
                        Data = await MailList(request.Filter)
                    };

                }



                return new EmailListCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = "Reference to selected email list couldn't be resolved.",
                        Result = RequestResult.Warn,
                        Succeeded = false,
                        Title = "Empty List"
                    },
                    Data = await MailList(request.Filter)
                };




            }
            catch (Exception ex)
            {

                return new EmailListCRUDResponse
                {
                    Response = new BaseResponse
                    {
                        Message = ex.Message,
                        Result = RequestResult.Error,
                        Succeeded = false,
                        Title = ex.Source
                    },
                    Data = await MailList(request.Filter)
                };
            }
        }

        private async Task<List<EmailListResponse>> TopMost(Pagination pagination)
        {
            try
            {
                return await _context.EmailLists.OrderByDescending(e => e.CreatedOn)
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize)
                    .Select(e => new EmailListResponse()
                {
                    Id = e.Id,
                    EmailAddress = e.EmailAddress,
                    FirstName = string.IsNullOrEmpty(e.FirstName) ? string.Empty : e.FirstName,
                    LastName = string.IsNullOrEmpty(e.LastName) ? string.Empty : e.LastName,
                    SignupDate = e.CreatedOn.ToString(DefaultDateFormat.ddMMyyyy)


                }).ToListAsync();
            }
            catch (Exception)
            {

                return new List<EmailListResponse>();
            }
        }
    }
}
