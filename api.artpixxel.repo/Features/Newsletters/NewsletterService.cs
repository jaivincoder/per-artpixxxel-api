
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.Newsletters;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Newsletters
{
    public class NewsletterService : INewsletterService
    {
        private readonly ArtPixxelContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        public NewsletterService(ArtPixxelContext context,  ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        private async Task<BaseResponse> AddToEmailList(User user)
        {
            try
            {
                EmailList _emailList = new()
                {
                    EmailAddress = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                _context.EmailLists.Add(_emailList);
                int addResult = await _context.SaveChangesAsync();
                if (addResult > 0)
                {

                    return new BaseResponse
                    {
                        Message = "Newsletter subscription updated.",
                        Result = RequestResult.Success,
                        Succeeded = true,
                        Title = "Successful"
                    };
                }

                return new BaseResponse
                {
                    Message = "Newsletter subscription failed.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Unknown Error"
                };

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task UpdateCustomerSubscription(NewsletterSubscription request)
        {
            Customer customer = await _context.Customers.Where(e => e.UserId == _currentUserService.GetUserId()).FirstOrDefaultAsync();
            if (customer != null)
            {
                customer.SubscribedNewsletter = @request.Receiving;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BaseResponse> Subscription(NewsletterSubscription request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {
                    User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                    if (user != null)
                    {
                        if (@request.Receiving)
                        {
                            if (await _context.EmailLists.IgnoreQueryFilters().AnyAsync(e => e.EmailAddress == user.Email))
                            {
                                EmailList emailList = await _context.EmailLists.IgnoreQueryFilters().Where(e => e.EmailAddress == user.Email).FirstOrDefaultAsync();
                                if (emailList != null)
                                {
                                    emailList.IsDeleted = false;
                                    emailList.DeletedBy = null;
                                    emailList.DeletedOn = null;

                                    _context.EmailLists.Update(emailList);
                                    int updateResult = await _context.SaveChangesAsync();

                                   await UpdateCustomerSubscription(@request);


                                    if (updateResult > 0)
                                    {
                                        return new BaseResponse
                                        {
                                            Message = "Newsletter subscription updated.",
                                            Result = RequestResult.Success,
                                            Succeeded = true,
                                            Title = "Successful"
                                        };
                                    }


                                }

                                else
                                {

                                    await UpdateCustomerSubscription(@request);
                                    return await AddToEmailList(user);

                                }
                            }
                            else
                            {
                                await UpdateCustomerSubscription(@request);
                                return await AddToEmailList(user);
                            }
                        }


                        else
                        {
                            if (await _context.EmailLists.AnyAsync(e => e.EmailAddress == user.Email))
                            {
                                EmailList emailList = await _context.EmailLists.Where(e => e.EmailAddress == user.Email).FirstOrDefaultAsync();
                                if (emailList != null)
                                {
                                    _context.EmailLists.Remove(emailList);
                                    await _context.SaveChangesAsync();

                                    await UpdateCustomerSubscription(@request);

                                    return new BaseResponse
                                    {
                                        Message = "Newsletter unsubscription succeeded.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful"
                                    };


                                }

                            }



                            return new BaseResponse
                            {
                                Message = "You are currently not subscribed to our newsletter.",
                                Result = RequestResult.Error,
                                Succeeded = false,
                                Title = "Invalid Request"
                            };

                        }
                    }

                }

                return new BaseResponse
                {
                    Message = "We were unable to verify this account.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Authentication Failure"
                };
            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source
                };
            }
        }

        public async Task<BaseResponse> Subscribe(NewsletterRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);


                if (await _context.EmailLists.IgnoreQueryFilters().AnyAsync(e => e.EmailAddress == @request.EmailAddress && (e.IsDeleted == false)))
                {
                    return new BaseResponse
                    {
                        Message = "You have already subscribed to our newsletter.",
                        Result = RequestResult.Warn,
                        Succeeded = true,
                        Title = "Duplicate Subscription"
                    };
                }
                else if (await _context.EmailLists.IgnoreQueryFilters().AnyAsync(e => e.EmailAddress == @request.EmailAddress && (e.IsDeleted == true)))
                {
                    EmailList emailList = await _context.EmailLists.IgnoreQueryFilters().Where(e => e.EmailAddress == @request.EmailAddress && (e.IsDeleted == true)).FirstOrDefaultAsync();
                    if (emailList != null)
                    {
                        emailList.IsDeleted = false;
                        emailList.DeletedBy = null;
                        emailList.DeletedOn = null;

                        _context.EmailLists.Update(emailList);
                        int updateResult = await _context.SaveChangesAsync();

                      


                        if (updateResult > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Newsletter subscription updated.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        }


                    }

                    else
                    {


                        EmailList _emailList = new()
                        {
                            EmailAddress = @request.EmailAddress,
                           
                        };

                        _context.EmailLists.Add(_emailList);
                        int addResult = await _context.SaveChangesAsync();

                        if(addResult > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Newsletter subscription succeeded.",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
                            };
                        }

                    }
                }


                else
                {
                    EmailList _emailList = new()
                    {
                        EmailAddress = @request.EmailAddress,

                    };

                    _context.EmailLists.Add(_emailList);
                    int addResult = await _context.SaveChangesAsync();

                    if (addResult > 0)
                    {
                        return new BaseResponse
                        {
                            Message = "Newsletter subscription succeeded.",
                            Result = RequestResult.Success,
                            Succeeded = true,
                            Title = "Successful"
                        };
                    }
                }


                return new BaseResponse
                {
                    Message = "Newsletter subscription failed. Please try again later.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Newsletter Subscription Error"
                };

            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source
                };
            }
        }

        public async Task<BaseBoolResponse> Exists(NewsletterRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.EmailLists.IgnoreQueryFilters().AnyAsync(e => e.EmailAddress == @request.EmailAddress && (e.IsDeleted == false)))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "<b>" + @request.EmailAddress + " </b> already exists in our email list."
                    };
                }


                return new BaseBoolResponse
                {
                    Exist = false,
                    Message = string.Empty
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
    }
}
