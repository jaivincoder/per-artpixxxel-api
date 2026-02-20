

using api.artpixxel.data.Models;
using api.artpixxel.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Seed
{
    public static class UserSeed
    {
        public static async Task Initialize(
            ArtPixxelContext context,
            UserManager<User> userManager,
            RoleManager<UserRole> roleManager,
            IWebHostEnvironment hosting)
        {
            context.Database.EnsureCreated();


            if (await roleManager.FindByNameAsync(DefaultRoles.SuperAdmin) == null)
            {
                await roleManager.CreateAsync(new UserRole(DefaultRoles.SuperAdmin, DefaultRoleDescripion.SuperAdminDescription, DateTime.Now, DefaultRoles.SuperAdmin));
            }

            if (await roleManager.FindByNameAsync(DefaultRoles.Admin) == null)
            {
                await roleManager.CreateAsync(new UserRole(DefaultRoles.Admin, DefaultRoleDescripion.AdminDescription, DateTime.Now, DefaultRoles.SuperAdmin));
            }

            if (await roleManager.FindByNameAsync(DefaultRoles.Employee) == null)
            {
                await roleManager.CreateAsync(new UserRole(DefaultRoles.Employee, DefaultRoleDescripion.EmployeeDescription, DateTime.Now, DefaultRoles.SuperAdmin));
            }

            if (await roleManager.FindByNameAsync(DefaultRoles.Customer) == null)
            {
                await roleManager.CreateAsync(new UserRole(DefaultRoles.Customer, DefaultRoleDescripion.CustomerDescription, DateTime.Now, DefaultRoles.SuperAdmin));
            }


            if (await userManager.FindByEmailAsync(DefaultEmails.SuperAdminEmail) == null)
            {
                var user = new User
                {
                    UserName = DefaultUserNames.SuperAdmin,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = DefaultEmails.SuperAdminEmail,
                    FirstName = "Gbenga",
                    MiddleName = "",
                    LastName = "Ogundare",
                    PhoneNumber = "08033071452",
                    HomeAddress = "Lagos, Nigeria",
                    Gender = Gender.Male,
                    IsAdmin = false,
                    EmailConfirmed = true,
                    LockoutEnabled = false

                };
                var result = await userManager.CreateAsync(user, DefaultPassword.SuperAdminDefault);
                if (result.Succeeded)
                {

                    await userManager.AddToRoleAsync(user, DefaultRoles.SuperAdmin);

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Email),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                            new Claim(ClaimTypes.Role,  DefaultRoles.SuperAdmin)
                        };
                    await userManager.AddClaimsAsync(user, claims);
                    await roleManager.SeedClaimsForSuperAdmin();


                }


            }


            if (await userManager.FindByEmailAsync(DefaultEmails.AdminEmail) == null)
            {
                var user = new User
                {
                    UserName = DefaultUserNames.Admin,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = DefaultEmails.AdminEmail,
                    FirstName = "Yemi",
                    MiddleName = "",
                    LastName = "Ogunlolu",
                    PhoneNumber = "+1(301)543-0262",
                    HomeAddress = "",
                    Gender = Gender.Male,
                    EmailConfirmed = true,
                    IsAdmin = true,
                    LockoutEnabled = false

                };
                var result = await userManager.CreateAsync(user, DefaultPassword.AdminDefault);
                if (result.Succeeded)
                {

                    await userManager.AddToRoleAsync(user, DefaultRoles.Admin);

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Email),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                            new Claim(ClaimTypes.Role, DefaultRoles.Admin)
                        };
                    await userManager.AddClaimsAsync(user, claims);
                    await roleManager.SeedClaimsForAdmin();


                }


            }

        }



        private async static Task SeedClaimsForSuperAdmin(this RoleManager<UserRole> roleManager)
        {
            var superAdminRole = await roleManager.FindByNameAsync(DefaultRoles.SuperAdmin);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.CustomerType);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.Employee);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.HomeSlider);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.MixMatch);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.MixMatchCategory);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.Order);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.OrderStatus);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.Permission);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.State);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.UserRole);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.Country);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.WallArt);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.WallArtCategory);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.WallArtSize);
            await roleManager.AddCustomPermissionClaim(superAdminRole, CustomPermission.Frame);
            
          
        }


        private async static Task SeedClaimsForAdmin(this RoleManager<UserRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(DefaultRoles.Admin);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.CustomerType);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.Employee);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.HomeSlider);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.MixMatch);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.MixMatchCategory);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.Order);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.OrderStatus);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.Permission);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.State);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.UserRole);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.Country);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.WallArt);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.WallArtCategory);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.WallArtSize);
            await roleManager.AddCustomPermissionClaim(adminRole, CustomPermission.Frame);



        }





        public static async Task AddCustomPermissionClaim(this RoleManager<UserRole> roleManager, UserRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }



        public static async Task AddCustomUserPermissionClaim(this UserManager<User> userManager, User user, string module)
        {
            var allClaims = await userManager.GetClaimsAsync(user);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await userManager.AddClaimAsync(user, new Claim("Permission", permission));
                }
            }
        }



    }
}
