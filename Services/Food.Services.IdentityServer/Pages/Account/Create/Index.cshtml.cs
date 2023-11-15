using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using Food.Services.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using static FoodOrderApp.Pages.Login.ViewModel;
using static IdentityModel.OidcConstants;

namespace FoodOrderApp.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    [BindProperty]
    public InputModel Input { get; set; }
    [BindProperty]
    public List<SelectListItem> Roles { get; set; }
    public Index(
        IIdentityServerInteractionService interaction,
        IEventService events,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _interaction = interaction;
        _events = events;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> OnGet(string returnUrl)
    {
        //Input = new InputModel { ReturnUrl = returnUrl };
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        List<SelectListItem> roles = new List<SelectListItem>();
        roles.Add(new SelectListItem("Admin", "Admin"));
        roles.Add(new SelectListItem("Customer", "Customer"));
        Roles = roles;// new SelectList(roles);
        Input = new InputModel { ReturnUrl = returnUrl };
        //Input.Roles = roles;
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        // the user clicked the "cancel" button
        if (Input.Button != "create")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl);
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }
        }
        //if (_users.FindByUsername(Input.Username) != null)
        var exstnguser = await _userManager.FindByNameAsync(Input.Username);
        if (exstnguser?.Id != null)
        {
            ModelState.AddModelError("Input.Username", "Invalid username");
        }

        //if (ModelState.IsValid)
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true,
                FirstName = Input.FirstName,
                LastName = Input.LastName
            };

            var newuser = await _userManager.CreateAsync(user, Input.Password);
            
            if (!_roleManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
            {
                var userRole = new IdentityRole
                {
                    Name = Input.RoleName,
                    NormalizedName = Input.RoleName,

                };
                await _roleManager.CreateAsync(userRole);
            }

            await _userManager.AddToRoleAsync(user, Input.RoleName);

            await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim(JwtClaimTypes.Name, Input.Username),
                            new Claim(JwtClaimTypes.Email, Input.Email),
                            new Claim(JwtClaimTypes.FamilyName, Input.FirstName),
                            new Claim(JwtClaimTypes.GivenName, Input.LastName),
                            new Claim(JwtClaimTypes.WebSite, "http://"+Input.Username+".com"),
                            new Claim(JwtClaimTypes.Role,"User") });

            var loginresult = await _signInManager.PasswordSignInAsync(
                Input.Username, Input.Password, false, lockoutOnFailure: true);

            if (loginresult.Succeeded)
            {

                //var checkuser = await _userManager.FindByNameAsync(Input.Username);
                //await _events.RaiseAsync(new UserLoginSuccessEvent(checkuser.UserName, checkuser.Id, checkuser.UserName, clientId: context?.Client.ClientId));

                // issue authentication cookie with subject ID and username
                var isuser = new IdentityServerUser(user.Id)
                {
                    DisplayName = user.FirstName + " " + user.LastName
                };

                //await HttpContext.SignInAsync(isuser);

                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(Input.ReturnUrl);
                }

                // request for a local page
                if (Url.IsLocalUrl(Input.ReturnUrl))
                {
                    return Redirect(Input.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(Input.ReturnUrl))
                {
                    return Redirect("~/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }
            }

            return Page();
        }
        else
        {
            throw new Exception("Login not succeeded");
        }
    }
}