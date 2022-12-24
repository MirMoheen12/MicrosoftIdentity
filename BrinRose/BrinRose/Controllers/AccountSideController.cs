using BrinRose.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrinRose.Controllers
{
    [AllowAnonymous]
    public class AccountSideController : Controller
    {
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> RoleManager;
        public AccountSideController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            RoleManager = roleManager;
        }
        public async Task CreateRole()
        {

            bool x = await RoleManager.RoleExistsAsync("Admin");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                await RoleManager.CreateAsync(role);
            }
            bool y = await RoleManager.RoleExistsAsync("Moderator");
            if (!y)
            {
                var role = new IdentityRole();
                role.Name = "Moderator";
                await RoleManager.CreateAsync(role);
            }
        }
        //public IActionResult AddUser()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> AddUser(RegisterationModel registerationModel)
        //{
        //    var data = new IdentityUser { UserName = registerationModel.Email, Email = registerationModel.Email };
        //    var res = await userManager.CreateAsync(data, registerationModel.Password);
        //    if (res.Succeeded)
        //    {
        //        if (!await RoleManager.RoleExistsAsync("Admin"))
        //            await signInManager.SignInAsync(data, isPersistent: false);
        //        var user = new IdentityRole("Admin");
        //        var r = await RoleManager.CreateAsync(user);
        //        if (r.Succeeded)
        //        {
        //            await userManager.AddToRoleAsync(data, "Admin");
        //        }

        //    }
        //    return View();
        //}
        //public IActionResult Login()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel loginModel)
        //{
        //    var result = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, false);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Index", "AdminSide");
        //    }
        //    return View();
        //}

        public async Task<IActionResult> Login(string returnUrl)
        {
            await signInManager.SignOutAsync();
            LoginModel loginViewModel = new LoginModel
            {
                ReturnUrl = returnUrl,
                Externallogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            ViewBag.loginViewModel = loginViewModel;
            return View();
        }
 
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = Url.Action("ExternalLoginCallBack", "AccountSide", new { RetunrUrl = returnUrl });
            var prop = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);
            return new ChallengeResult(provider, prop);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
      
        public async Task<IActionResult> ExternalLoginCallBack(string RetunrUrl = null, string remoteError = null)
        {
            RetunrUrl = RetunrUrl ?? Url.Content("~/");
            LoginModel loginViewModel = new LoginModel
            {
                ReturnUrl = RetunrUrl,
                Externallogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(String.Empty, $"Error from External Provide:{remoteError}");
                return View("Login", loginViewModel);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(String.Empty, $"Error from External Login info:{remoteError}");
                return View("Login", loginViewModel);
            }
            var signinresult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signinresult.Succeeded)
            {
              return RedirectToAction("Index", "Home");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Name);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Name),
                           
                        };
                        await userManager.CreateAsync(user);
                        var defaultrole = RoleManager.FindByNameAsync("Admin").Result;
                        if (defaultrole != null)
                        {
                                IdentityResult roleresult = await userManager.AddToRoleAsync(user, defaultrole.Name);
                        }
                        else
                        {
                            await CreateRole();
                            defaultrole = RoleManager.FindByNameAsync("Admin").Result;
                            IdentityResult roleresult = await userManager.AddToRoleAsync(user, defaultrole.Name);
                        }
                    }
                    await userManager.AddLoginAsync(user, info);

                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ErrorTtle = $"Email not Found:{info.LoginProvider}";
                return View("Error");

            }

        }

    }
}
