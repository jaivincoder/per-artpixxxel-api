

using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.AddressBooks;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Emails;
using api.artpixxel.data.Features.Notifications;
using api.artpixxel.data.Features.SMTPs;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Extensions;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly ArtPixxelContext _context;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly AppKeyConfig _appkeys;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISMTPService _sMTPService;
        public AccountService(
            ArtPixxelContext context,
            UserManager<User> userManager,
            IOptions<AppKeyConfig> appkeys,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            IWebHostEnvironment hostingEnvironment,
            RoleManager<UserRole> roleManager,
            ISMTPService sMTPService,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _notificationService = notificationService;
            _currentUserService = currentUserService;
            _appkeys = appkeys.Value;
            _sMTPService = sMTPService;
        }

        public async Task<BaseResponse> CreateCustomer(AccountCreateRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (!await _context.Users.AnyAsync(e => e.Email == @request.EmailAddress || (e.PhoneNumber == @request.MobileNumber) || (e.UserName == @request.Username)))
                {
                    User user = new()
                    {
                        Email = @request.EmailAddress,
                        UserName = @request.Username,
                        FirstName = @request.FirstName,
                        LastName = @request.LastName,
                        PhoneNumber = @request.MobileNumber

                    };

                    IdentityResult result = await _userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
                        List<Claim> userClaims = new()
                        {
                            new Claim(ClaimTypes.Role, DefaultRoles.Customer),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Surname, user.LastName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
                        };

                        await _userManager.AddClaimsAsync(user, userClaims);


                        string categoryId = null;
                        if (await _context.CustomerCategories.AnyAsync(e => e.IsDefault == true))
                        {
                            CustomerCategory customerCategory = await _context.CustomerCategories.Where(e => e.IsDefault == true).FirstOrDefaultAsync();
                            if (customerCategory != null)
                            {
                                categoryId = customerCategory.Id;
                            }
                        };

                        Customer customer = new()
                        {
                            UserId = _context.Users.Find(user.Id).Id,
                            CategoryId = categoryId,
                            SubscribedNewsletter = @request.Newsletter

                        };


                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();



                        if (@request.Newsletter == true)
                        {
                            await _emailService.AddToEmailList(new EmailListRequest
                            {
                                EmailAddress = @request.EmailAddress,
                                FirstName = @request.FirstName,
                                LastName = @request.LastName
                            });
                        }

                        //send Welcome email

                        List<string> admins = await _context.Users.Where(e => e.IsAdmin == true).Select(e => e.Id).ToListAsync();
                        if (admins.Any())
                        {
                            await _notificationService.SendNotification(
                                     new NotificationModel
                                     {
                                         Recipients = admins,
                                         NotificationRequest = new NotificationContent
                                         {
                                             AccessType = AccessType.Specific,
                                             SubjectId = user.Id,
                                             NotificationPriority = NotificationPriority.Default,
                                             Message = user.FullName + " created a customer account.",
                                             Title = "New Customer Signup"

                                         }
                                     });
                        }


                        await _sMTPService.SendWelcomeMail(new AccountModel
                        {
                            EmailAddress = @request.EmailAddress,
                            FirstName = @request.FirstName,
                            LastName = @request.LastName,
                            Username = @request.Username,
                            Password = @request.Password
                        });





                        return new BaseResponse
                        {
                            Message = "Account created",
                            Title = "Successful",
                            Result = RequestResult.Success,
                            Succeeded = true
                        };
                    }
                }



                return new BaseResponse
                {
                    Title = "Duplicate Account",
                    Message = "Account already exists",
                    Result = RequestResult.Error,
                    Succeeded = false
                };
            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Title = ex.Source,
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false
                };
            }
        }

        public async Task<BaseBoolResponse> Exists(DuplicateAccount request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Users.AnyAsync(e => e.Email == @request.EmailAddress))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Email address: <i>" + @request.EmailAddress + " </i> already taken"
                    };
                }

                if (await _context.Users.AnyAsync(e => e.PhoneNumber == @request.MobileNumber))
                {
                    return new BaseBoolResponse
                    {
                        Exist = true,
                        Message = "Mobile number: <i>" + @request.MobileNumber + " </i> already taken"
                    };
                }

                if (!string.IsNullOrWhiteSpace(@request.Username))
                {
                    if (await _context.Users.AnyAsync(e => e.UserName == @request.Username))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = "Username: <i>" + @request.Username + " </i> already taken"
                        };
                    }

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




        private async Task<string> GenerateJwtToken(User user)
        {



            if (!await _context.Customers.AnyAsync(e => e.UserId == user.Id))
            {
                return null;
            }
            else
            {



                IList<string> roles = await _userManager.GetRolesAsync(user);
                string rolesString = string.Join(",", roles);
                List<string> _permissions = new();
                List<Claim> claims = new();
                List<string> allClaimValues = new();
                List<string> roleClaimValues = new();
                List<string> authorizedClaims = new();


                foreach (var role in roles)
                {
                    var usRole = await _roleManager.FindByNameAsync(role);



                    var clms = await _roleManager.GetClaimsAsync(usRole);
                    var userClms = await _userManager.GetClaimsAsync(user);
                    if (userClms.Any())
                    {
                        claims.AddRange(userClms.ToList());
                    }

                    if (clms.Any())
                    {
                        claims.AddRange(clms.ToList());
                    }


                }






                claims = claims.Distinct().ToList();





                IdentityOptions _options = new IdentityOptions();
                JwtSecurityTokenHandler tokenHandler = new();

                var key = Encoding.ASCII.GetBytes(_appkeys.Secret);
                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim(_options.ClaimsIdentity.RoleClaimType, rolesString),


                }),

                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };



                var token = tokenHandler.CreateToken(tokenDescriptor);
                var encryptedtoken = tokenHandler.WriteToken(token);

                return encryptedtoken;
            }
        }



        public async Task<LoginResponse> Login(LoginRequest model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.UserName);
                if (user == null)
                {
                    return new LoginResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Unrecognized Login Attempt",
                        Message = "Account not found. Please check your email address",
                    };

                }

                var PasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!PasswordValid)
                {
                    return new LoginResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "Please check password and try again.",
                        Title = "Wrong Password"
                    };

                }

                var encryptedtoken = await GenerateJwtToken(user);
                if (!string.IsNullOrEmpty(encryptedtoken))
                {

                    user.LastLogin = DateTime.UtcNow;
                    user.IsOnline = true;
                    await _userManager.UpdateAsync(user);
                }


                return new LoginResponse
                {
                    Token = encryptedtoken,
                    Succeeded = string.IsNullOrEmpty(encryptedtoken) ? false : true,
                    Result = string.IsNullOrEmpty(encryptedtoken) ? RequestResult.Error : RequestResult.Success,
                    Title = string.IsNullOrEmpty(encryptedtoken) ? "Authentication Error" : "Authentication Successful",
                    Message = string.IsNullOrEmpty(encryptedtoken) ? "An authentication error occurred. User authentication failed " : "Login Successful"
                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BaseResponse> ResetPassword(PasswordReset reset)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@reset", reset);

                if (await _context.Users.AnyAsync(e => e.Id == reset.UserId))
                {
                    User user = await _userManager.FindByIdAsync(@reset.UserId);
                    if (user != null)
                    {
                        if (await _userManager.CheckPasswordAsync(user, reset.NewPassword))
                        {
                            return new BaseResponse
                            {
                                Title = "Redundant Request",
                                Result = RequestResult.Error,
                                Message = "New password same as current password",
                                Succeeded = false,
                            };
                        }

                        else
                        {
                            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                            IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, resetToken, reset.NewPassword);
                            if (resetResult.Succeeded)
                            {
                                if (reset.NotifyUser)
                                {
                                    //send email notification

                                    return new BaseResponse
                                    {
                                        Title = "Successful",
                                        Result = RequestResult.Success,
                                        Message = "Password change successful",
                                        Succeeded = true,
                                    };
                                }

                                else
                                {
                                    return new BaseResponse
                                    {
                                        Title = "Successful",
                                        Result = RequestResult.Success,
                                        Message = "Password change successful",
                                        Succeeded = true,
                                    };
                                }
                            }

                            else
                            {
                                return new BaseResponse
                                {
                                    Title = resetResult.Errors.ElementAt(0).Code,
                                    Result = RequestResult.Error,
                                    Message = resetResult.Errors.ElementAt(0).Description,
                                    Succeeded = resetResult.Succeeded
                                };
                            }

                        }

                    }
                }

                return new BaseResponse
                {
                    Message = "Reference to employee couldn't be resolved. This employee may have been deleted",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Null Customer Reference"
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

        public async Task<CustomerInfoData> Customer()
        {
            try
            {
                if (await _context.Customers.AnyAsync(e => e.UserId == _currentUserService.GetUserId()))
                {
                    Customer customer = await _context.Customers
                        .Where(e => e.UserId == _currentUserService.GetUserId())
                        .Include(e => e.User).ThenInclude(s => s.State).ThenInclude(c => c.Country).ThenInclude(f => f.Flag)
                        .Include(ad => ad.AddressBooks).ThenInclude(s => s.State)
                        .Include(a => a.AddressBooks).ThenInclude(c => c.Country).ThenInclude(f => f.Flag)
                        .FirstOrDefaultAsync();
                    if (customer != null)
                    {

                        return new CustomerInfoData
                        {
                            Customer = new CustomerInfo
                            {
                                Id = customer.Id,
                                FirstName = customer.User.FirstName,
                                LastName = customer.User.LastName,
                                Username = customer.User.UserName,
                                AdditionalMobileNumber = customer.AdditionalMobileNumber,
                                EmailAddress = customer.User.Email,
                                Newsletter = customer.SubscribedNewsletter,
                                AddressBooks = customer.AddressBooks.Any() ? customer.AddressBooks.Select(ad => new CustomerAddressBook
                                {
                                    Address = ad.Address,
                                    AdditionalInformation = ad.AdditionalInformation,
                                    City = string.IsNullOrEmpty(ad.CityName) ? new BaseOption { Id = string.Empty, Name = string.Empty } : new BaseOption { Id = Guid.NewGuid().ToString(), Name = ad.CityName },
                                    Country = string.IsNullOrEmpty(ad.CountryId) ? new CountryOption { Id = "", Name = "", Flag = "" } :
                                    new CountryOption { Id = ad.Country.Id, Name = ad.Country.Name, Flag = ad.Country.Flag.Name },
                                    State = string.IsNullOrEmpty(ad.StateId) ? new BaseOption { Id = "", Name = "" } : new BaseOption { Id = ad.State.Id, Name = ad.State.StateName },
                                    CustomerId = customer.Id,
                                    Id = ad.Id,
                                    IsDefault = ad.IsDefault,
                                    Name = ad.Name

                                }).ToList() : new List<CustomerAddressBook>(),
                                Dob = customer.User.DOB == null ? "" : customer.User.DOB.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyy),
                                Gender = customer.User.Gender.ToString(),
                                MobileNumber = customer.User.PhoneNumber,
                                Photo = string.IsNullOrEmpty(customer.User.PassportURL) ? "" : customer.User.PassportAbsURL,


                            },
                            Cities = await _context.Cities.Select(c => new BaseOption { Id = c.Id, Name = c.CityName }).ToListAsync(),
                            Countries = await _context.Countries.Include(f => f.Flag).Select(cc => new CountryOption { Id = cc.Id, Name = cc.Name, Flag = cc.Flag.Name }).ToListAsync(),
                            States = await _context.States.Select(s => new BaseOption { Id = s.Id, Name = s.StateName }).ToListAsync(),
                            Notifications = decimal.Round(await _context.UserMessages.Where(e => e.UserId == _currentUserService.GetUserId() && (e.ReadStatus == ReadStatus.Unread || e.Delivered == false)).CountAsync(), 0, MidpointRounding.AwayFromZero)
                        };

                    }
                }

                return new CustomerInfoData();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<BaseBoolResponse> Duplicate(DuplicateAccount request)
        {
            try
            {
                if (await _context.Customers.AnyAsync(e => e.UserId == _currentUserService.GetUserId()))
                {
                    SqlParameter[] myparm = new SqlParameter[1];
                    myparm[0] = new SqlParameter("@request", request);

                    if (await _context.Users.AnyAsync(e => e.Email == @request.EmailAddress && (e.Id != _currentUserService.GetUserId())))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = "Email address: <i>" + @request.EmailAddress + " </i> already taken"
                        };
                    }

                    if (await _context.Users.AnyAsync(e => e.PhoneNumber == @request.MobileNumber && (e.Id != _currentUserService.GetUserId())))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = "Mobile number: <i>" + @request.MobileNumber + " </i> already taken"
                        };
                    }


                    if (await _context.Users.AnyAsync(e => e.UserName == @request.Username && (e.Id != _currentUserService.GetUserId())))
                    {
                        return new BaseBoolResponse
                        {
                            Exist = true,
                            Message = "Username: <i>" + @request.Username + " </i> already taken"
                        };
                    }



                    return new BaseBoolResponse
                    {
                        Exist = false,
                        Message = string.Empty
                    };
                }

                return new BaseBoolResponse
                {
                    Exist = true,
                    Message = "Customer Authentication Failed"
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

        public async Task<BaseResponse> UpdateCustomer(CustomerDetail request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Customers.AnyAsync(e => e.UserId == _currentUserService.GetUserId()))
                {
                    Customer customer = await _context.Customers
                        .Where(e => e.UserId == _currentUserService.GetUserId())
                        .Include(e => e.User)
                        .FirstOrDefaultAsync();
                    if (customer != null)
                    {
                        string outputPath = string.Empty;
                        FileMeta fileMeta = new() { Path = null, FileName = null, ImageByte = null };




                        if (string.IsNullOrEmpty(customer.User.PassportURL))
                        {

                            if (@request.Photo.IsBase64String())
                            {
                                outputPath = _hostingEnvironment.WebRootPath + "\\images\\Customer\\" + @request.Username;
                                fileMeta = await @request.Photo.SaveBase64AsImage(outputPath);
                            }


                        }

                        else
                        {

                            if (@request.Photo.IsBase64String())
                            {
                                outputPath = _hostingEnvironment.WebRootPath + "\\images\\Customer\\" + @request.Username;
                                fileMeta = await customer.User.PassportURL.RenameFile(@request.Photo, outputPath);
                            }

                            else if (@request.Photo == AssetDefault.DefaultUserImage)
                            {
                                await customer.User.PassportURL.DeleteFileFromPathAsync();
                                customer.User.PassportURL = null;
                                customer.User.PassportAbsURL = null;
                                customer.User.PassportRelURL = null;
                            }


                        }






                        if (!string.IsNullOrEmpty(request.AdditionalMobileNumber))
                        {
                            if (customer.AdditionalMobileNumber != @request.AdditionalMobileNumber)
                            {
                                customer.AdditionalMobileNumber = @request.AdditionalMobileNumber;


                            }
                        }

                        customer.User.FirstName = @request.FirstName;
                        customer.User.LastName = @request.LastName;
                        customer.User.Email = @request.EmailAddress;
                        customer.User.UserName = @request.Username;
                        customer.User.PhoneNumber = @request.MobileNumber;
                        customer.User.Gender = (Gender)Enum.Parse(typeof(Gender), @request.Gender);
                        customer.User.PassportURL = string.IsNullOrEmpty(fileMeta.Path) ? customer.User.PassportURL : fileMeta.Path;
                        customer.User.PassportAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? customer.User.PassportAbsURL : _currentUserService.WebRoot() + "/images/Customer/" + fileMeta.FileName;
                        customer.User.PassportRelURL = string.IsNullOrEmpty(fileMeta.Path) ? customer.User.PassportRelURL : "/images/Customer/" + fileMeta.FileName;
                        customer.User.DOB = string.IsNullOrEmpty(request.Dob) ? null : DateTime.ParseExact(request.Dob, DefaultDateFormat.ddMMyyyy, CultureInfo.InvariantCulture);



                        _context.Customers.Update(customer);
                        int saveResult = await _context.SaveChangesAsync();
                        if (saveResult > 0)
                        {
                            return new BaseResponse
                            {
                                Message = "Account details update succeded",
                                Result = RequestResult.Success,
                                Succeeded = true,
                                Title = "Successful"
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

        public async Task<BaseResponse> ResetSelfPassword(SelfPasswordReset request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {
                    User user = await _userManager.FindByIdAsync(_currentUserService.GetUserId());
                    if (user != null)
                    {
                        if (await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
                        {

                            if (await _userManager.CheckPasswordAsync(user, request.NewPassword))
                            {
                                return new BaseResponse
                                {
                                    Title = "Redundant Request",
                                    Result = RequestResult.Error,
                                    Message = "New password same as current password",
                                    Succeeded = false,
                                };
                            }

                            else
                            {
                                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                                IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, resetToken, @request.NewPassword);
                                if (resetResult.Succeeded)
                                {

                                    return new BaseResponse
                                    {
                                        Title = "Successful",
                                        Result = RequestResult.Success,
                                        Message = "Password change successful",
                                        Succeeded = true,
                                    };

                                }

                                else
                                {
                                    return new BaseResponse
                                    {
                                        Title = resetResult.Errors.ElementAt(0).Code,
                                        Result = RequestResult.Error,
                                        Message = resetResult.Errors.ElementAt(0).Description,
                                        Succeeded = resetResult.Succeeded
                                    };
                                }
                            }

                        }

                        else
                        {
                            return new BaseResponse
                            {
                                Title = "Authentication Failure",
                                Result = RequestResult.Error,
                                Message = "Supplied current password not same as current password",
                                Succeeded = false,
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

        public async Task<CustomerNotice> Notices()
        {
            try
            {
                if (await _context.UserMessages.AnyAsync(e => e.UserId == _currentUserService.GetUserId() && (e.ReadStatus == ReadStatus.Unread || e.Delivered == false)))
                {
                    return new CustomerNotice
                    {
                        NewMessages = await _context.UserMessages.Where(e => e.UserId == _currentUserService.GetUserId() && (e.ReadStatus == ReadStatus.Unread || e.Delivered == false)).CountAsync()
                    };
                }

                return new CustomerNotice
                {
                    NewMessages = 0
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<LogoutResponse> Logout()
        {
            try
            {
                User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                if (user != null)
                {
                    user.IsOnline = false;
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return new LogoutResponse
                        {
                            LoggedIn = false
                        };
                    }


                }

                return new LogoutResponse
                {
                    LoggedIn = false
                };
            }
            catch (Exception)
            {

                return new LogoutResponse
                {
                    LoggedIn = false
                };
            }
        }

        public async Task<BaseResponse> ResetToken(ResetTokenRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Users.AnyAsync(e => e.Email == @request.EmailAddress))
                {
                    User user = await _userManager.FindByEmailAsync(request.EmailAddress);
                    if (user != null)
                    {
                        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                        if (resetToken != null)
                        {

                            user.Token = resetToken;
                            IdentityResult result = await _userManager.UpdateAsync(user);

                            if (result.Succeeded)
                            {



                                EmailSendStatus emailResult = await _sMTPService.SendPasswordResetToken(new TokentNotificationModel
                                {
                                    EmailAddress = user.Email,
                                    FullName = user.FullName,
                                    Token = user.Id
                                });

                                if (emailResult.Succeeded)
                                {
                                    return new BaseResponse
                                    {
                                        Message = "A one-time reset token has been sent to your email address: " + user.Email + ". Token expires in 30mins.",
                                        Result = RequestResult.Success,
                                        Succeeded = true,
                                        Title = "Successful",
                                    };

                                }

                                else
                                {

                                    return new BaseResponse
                                    {
                                        Message = "An error encountered sending token to the provided email. Please try again later.",
                                        Result = RequestResult.Error,
                                        Succeeded = false,
                                        Title = "Network Error",
                                    };

                                }
                            }



                        }


                        return new BaseResponse
                        {
                            Message = "An error encountered generating reset token. Please try again later.",
                            Result = RequestResult.Error,
                            Succeeded = false,
                            Title = "Network Error",
                        };



                    }
                }


                return new BaseResponse
                {
                    Message = "No user account associated with the email address: " + request.EmailAddress + ". Please provide a valid email",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Invalid Reference",
                };

            }
            catch (Exception ex)
            {

                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source,

                };
            }
        }

        public async Task<BaseResponse> NewPassword(PasswordResetRequest request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);
                User user = null;


                byte[] _tokenBytes = Convert.FromBase64String(request.Token);
                string _token = Encoding.UTF8.GetString(_tokenBytes);

                if (await _context.Users.AnyAsync(e => e.Id == _token))
                {
                    user = await _userManager.FindByIdAsync(_token);
                }





                if (user != null)
                {


                    IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, user.Token, @request.Password);

                    if (resetResult.Succeeded)
                    {

                        return new BaseResponse
                        {
                            Title = "Successful",
                            Result = RequestResult.Success,
                            Message = "Password change successful",
                            Succeeded = true,
                        };

                    }

                    else
                    {
                        return new BaseResponse
                        {
                            Title = resetResult.Errors.ElementAt(0).Code,
                            Result = RequestResult.Error,
                            Message = resetResult.Errors.ElementAt(0).Description,
                            Succeeded = resetResult.Succeeded
                        };
                    }
                }







                return new BaseResponse
                {
                    Message = "User account verification was not stressful. Password recovery failed.",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Invalid Request",
                };



            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = ex.Source,

                };

            }
        }
    }
}
