using Food.Services.IdentityServer.Data;
using Food.Services.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace Food.Services.IdentityServer.Initializer
{
    public static class DbInitializer
    {
        public static void Initialize(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            using var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            {
                using var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                {
                    if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
                    {
                        _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                        _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
                    }
                    else { return; }

                    ApplicationUser adminUser = new ApplicationUser()
                    {
                        UserName = @"admin1@gmail.com",
                        Email = "admin1@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "1111111111",
                        FirstName = "Sunandita",
                        LastName = "Das"
                    };

                    var result = _userManager.CreateAsync(adminUser, "Admin@123").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

                    var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[] {
                        new Claim(JwtClaimTypes.Name, adminUser.FirstName+" "+adminUser.LastName),
                        new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                        new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                        new Claim(JwtClaimTypes.Role, SD.Admin),
                    }).Result;

                    ApplicationUser customerUser = new ApplicationUser()
                    {
                        UserName = @"customer1@gmail.com",
                        Email = "customer1@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "1111111111",
                        FirstName = "Ben",
                        LastName = "Stock"
                    };

                    _userManager.CreateAsync(customerUser, "Customer@123").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

                    var temp2 = _userManager.AddClaimsAsync(adminUser, new Claim[] {
                new Claim(JwtClaimTypes.Name, adminUser.FirstName+" "+adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Admin),
            }).Result;
                }
            }
        }

    }
}
