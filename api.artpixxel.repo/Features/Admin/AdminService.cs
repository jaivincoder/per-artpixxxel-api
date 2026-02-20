

using api.artpixxel.data.Features.Accounts;
using api.artpixxel.data.Features.Admin;
using api.artpixxel.data.Features.Common;
using api.artpixxel.data.Features.Permission;
using api.artpixxel.data.Models;
using api.artpixxel.data.Services;
using api.artpixxel.Data;
using api.artpixxel.repo.Extensions;
using api.artpixxel.repo.Helpers;
using api.artpixxel.service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Features.Admin
{
    public class AdminService : IAdminServices
    {
        private readonly UserManager<User> _userManager;
        private readonly ArtPixxelContext _context;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly AppKeyConfig _appkeys;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly INotificationService _notificationService;
        public AdminService(
            UserManager<User> userManager,
            IOptions<AppKeyConfig> appkeys,
            RoleManager<UserRole> roleManager,
            ICurrentUserService currentUserService,
            IWebHostEnvironment hostingEnvironment,
            INotificationService notificationService,
            ArtPixxelContext context)
        {
            _userManager = userManager;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _roleManager = roleManager;
            _appkeys = appkeys.Value;
        }
        public async Task<BaseStringRequest> AllRoles()
        {
            try
            {
                List<UserRole> userRoles = await _roleManager.Roles.Where(rn => rn.Name != DefaultRoles.Customer).ToListAsync();
                string userRolesString = string.Join(",", userRoles);
                var key = Encoding.ASCII.GetBytes(_appkeys.Secret);
                JwtSecurityTokenHandler tokenHandler = new();
                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                    new Claim("allRoles", userRolesString)
                }),

                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var encryptedtoken = tokenHandler.WriteToken(token);

                return new BaseStringRequest
                {
                    Name = encryptedtoken
                };



            }
            catch (Exception)
            {

                throw;
            }
        }





        private async Task<string> GenerateJwtToken(User user)
        {



            if (await _context.Customers.AnyAsync(e => e.UserId == user.Id))
            {
                return null;
            }
            else
            {



                IList<string> roles = await _userManager.GetRolesAsync(user);
                string rolesString = string.Join(",", roles);
                List<string> _permissions = new();
                List<RoleClaimsModel> Permissions = new();
                List<Claim> claims = new();
                List<string> allClaimValues = new();
                List<string> roleClaimValues = new();
                List<string> authorizedClaims = new();
                

                foreach (var role in roles)
                {
                    var usRole = await _roleManager.FindByNameAsync(role);


                    Permissions.GetPermissions(typeof(Permissions.Country), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.CustomerType), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.Employee), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.HomeSlider), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.MixMatch), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.MixMatchCategory), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.Order), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.OrderStatus), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.Permission), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.State), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.UserRole), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.WallArt), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.WallArtCategory), usRole.Name);
                    Permissions.GetPermissions(typeof(Permissions.WallArtSize), usRole.Name);
                  
                  



                    var clms = await _roleManager.GetClaimsAsync(usRole);
                    var userClms = await _userManager.GetClaimsAsync(user);
                    if (userClms.Any())
                    {
                        claims.AddRange(userClms.ToList());
                    }
                    claims.AddRange(clms.ToList());

                }





                Permissions = Permissions.Distinct().ToList();
                claims = claims.Distinct().ToList();

                allClaimValues = Permissions.Select(a => a.Value).ToList();
                roleClaimValues = claims.Select(a => a.Value).ToList();
                authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();

                foreach (var Permission in Permissions)
                {
                    if (authorizedClaims.Any(a => a == Permission.Value))
                    {
                        _permissions.Add(Permission.Value);
                    }

                }
                string allPermissions = string.Join(",", _permissions.ToArray());
             

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
                    new Claim("permissions", allPermissions),
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
                User user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return new LoginResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Title = "Unrecognized Login Attempt",
                        Message = "You have two(2) more attempts.",
                    };

                }

                var PasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!PasswordValid)
                {
                    return new LoginResponse
                    {
                        Succeeded = false,
                        Result = RequestResult.Error,
                        Message = "You have four(4) more attempts",
                        Title = "Wrong Password"
                    };

                }

                user.LastLogin = DateTime.UtcNow;
                user.IsOnline = true;
                await _userManager.UpdateAsync(user);

                string encryptedtoken = await GenerateJwtToken(user);


                return new LoginResponse
                {
                    Token = encryptedtoken,
                    Succeeded = string.IsNullOrEmpty(encryptedtoken) ? false : true,
                    Result = string.IsNullOrEmpty(encryptedtoken) ? RequestResult.Error : RequestResult.Success,
                    Title = string.IsNullOrEmpty(encryptedtoken) ?  "Authentication Error" : "Authentication Successful",
                    Message = string.IsNullOrEmpty(encryptedtoken) ? "An authentication error occurred. User authentication failed " : "Login Successful"
                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AdminData> Admin()
        {
            try
            {
                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {


                    return new AdminData
                    {
                        UserInfo = await _context.Users.Where(e => e.Id == _currentUserService.GetUserId())
                        .Include(s => s.State)
                        .ThenInclude(c => c.Country)
                        .Select( u => new UserInfo
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            FirstName = u.FirstName,
                            MiddleName = u.MiddleName,
                            LastName = u.LastName,
                            EmailAddress = u.Email,
                            HomeAddress = u.HomeAddress,
                            Dob = u.DOB.HasValue ? u.DOB.GetValueOrDefault().ToString(DefaultDateFormat.ddMMyyyy) : null,
                            Username = u.UserName,
                            MobileNumber = u.PhoneNumber,
                            Photo = string.IsNullOrEmpty(u.PassportURL) ? AssetDefault.DefaultUserImage : u.PassportAbsURL,
                            Gender = u.Gender.ToString(),
                            Country = string.IsNullOrEmpty(u.StateId) ? null : u.State.Country.Id,
                            State = string.IsNullOrEmpty(u.StateId) ? null : u.State.Id

                        }).FirstOrDefaultAsync(),
                        Countries = await _context.Countries.Include(f => f.Flag).Select(cc => new CountryOption { Id = cc.Id, Name = cc.Name, Flag = cc.Flag.Name }).ToListAsync(),
                        States = await _context.States.Select(s => new BaseOption { Id = s.Id, Name = s.StateName }).ToListAsync(),
                        Notifications = await _notificationService.Notifications(),
                        Orders = decimal.Round( await _context.Orders.Where(e => e.Seen == false).CountAsync(), 0 ,MidpointRounding.AwayFromZero)
                    };
                }

                return new AdminData();
            }
            catch (Exception)
            {

                throw;
            }
        }



        public async Task<BaseBoolResponse> Duplicate(DuplicateAccount request)
        {
            try
            {
                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
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
                    Message = "User Authentication Failed"
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


        public async Task<PasswordCheckResponse> PasswordCheck(PasswordCheck request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                if (user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, @request.Password))
                    {
                        return new PasswordCheckResponse
                        {
                            SamePassword = true,
                            Message = string.Empty
                        };
                    }


                    return new PasswordCheckResponse
                    {
                        SamePassword = false,
                        Message = "Password: <i>" + @request.Password + " </i> different from current password"
                    };
                }

                return new PasswordCheckResponse
                {
                    SamePassword = false,
                    Message = "Athentication failure. User account couldn't be verified"
                };
            }
            catch (Exception ex)
            {

                return new PasswordCheckResponse
                {
                    SamePassword = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse> Update(UserInfoUpdate request)
        {
            try
            {
                SqlParameter[] myparm = new SqlParameter[1];
                myparm[0] = new SqlParameter("@request", request);

                if (await _context.Users.AnyAsync(e => e.Id == _currentUserService.GetUserId()))
                {
                    User user = await _userManager.GetUserAsync(_currentUserService.GetUser());
                    string outputPath = string.Empty;
                    FileMeta fileMeta = new() { FileName = null, ImageByte = null, Path = null };

                    if (user != null)
                    {
                        user.FirstName = @request.FirstName;
                        if (!string.IsNullOrEmpty(@request.MiddleName))
                        {
                            user.MiddleName = @request.MiddleName;
                        }
                        user.LastName = @request.LastName;
                        if (!string.IsNullOrEmpty(request.Dob))
                        {
                            user.DOB = @request.Dob.DDMMYYYY();
                        }
                        user.Email = @request.EmailAddress;
                        user.Gender = (Gender)Enum.Parse(typeof(Gender), @request.Gender);
                        user.HomeAddress = @request.HomeAddress;







                            if (string.IsNullOrEmpty(user.PassportURL))
                            {
                                if (@request.Photo.IsBase64String())
                                {
                                    outputPath = _hostingEnvironment.WebRootPath + "\\images\\Admins\\" + @request.Username;
                                    fileMeta = await @request.Photo.SaveBase64AsImage(outputPath);
                                    user.PassportURL = string.IsNullOrEmpty(fileMeta.Path) ? user.PassportURL : fileMeta.Path;
                                    user.PassportAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? user.PassportAbsURL : _currentUserService.WebRoot() + "/images/Admins/" + fileMeta.FileName;
                                    user.PassportRelURL = string.IsNullOrEmpty(fileMeta.Path) ? user.PassportRelURL : "/images/Admins/" + fileMeta.FileName;
                            }
                               
                            }

                            else
                            {

                                if (@request.Photo.IsBase64String())
                                {
                                    outputPath = _hostingEnvironment.WebRootPath + "\\images\\Admins\\" + @request.Username;
                                    fileMeta = await user.PassportURL.RenameFile(@request.Photo, outputPath);
                                    user.PassportURL = string.IsNullOrEmpty(fileMeta.Path) ? user.PassportURL : fileMeta.Path;
                                    user.PassportAbsURL = string.IsNullOrEmpty(fileMeta.Path) ? user.PassportAbsURL : _currentUserService.WebRoot() + "/images/Admins/" + fileMeta.FileName;
                                    user.PassportRelURL = string.IsNullOrEmpty(fileMeta.Path) ? user.PassportRelURL : "/images/Admins/" + fileMeta.FileName;
                            }



                                else if (@request.Photo == AssetDefault.DefaultUserImage)
                                {
                                    await user.PassportURL.DeleteFileFromPathAsync();
                                    user.PassportURL = null;
                                    user.PassportAbsURL = null;
                                    user.PassportRelURL = null;
                                }




                            }

                        

                       


                        user.PhoneNumber = @request.MobileNumber;
                        if (!string.IsNullOrEmpty(@request.State))
                        {
                            user.StateId = _context.States.Find(@request.State).Id;

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(user.StateId))
                            {
                                user.StateId = null;
                            }
                        }

                        user.UserName = @request.Username;

                        IdentityResult updateResult = await _userManager.UpdateAsync(user);
                        if (updateResult.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(@request.Password))
                            {
                                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                                IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, resetToken, @request.Password);
                                if (resetResult.Succeeded)
                                {
                                    return new BaseResponse
                                    {
                                        Title = "Successful",
                                        Message = "Profile updated.",
                                        Result = RequestResult.Success,
                                        Succeeded = true
                                    };
                                }
                            }


                            return new BaseResponse
                            {
                                Title = "Successful",
                                Message = "Profile updated.",
                                Result = RequestResult.Success,
                                Succeeded = true
                            };
                           
                        }

                       

                    }
                }

                return new BaseResponse
                {
                    Message = "User account verification failed",
                    Result = RequestResult.Error,
                    Succeeded = false,
                    Title = "Authentication Failure."
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
    }
}
