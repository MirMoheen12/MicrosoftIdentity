using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrinRose.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminSideController : Controller
    {
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> RoleManager;
        public AdminSideController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            RoleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Name);
            var re = await userManager.FindByEmailAsync(claimsIdentity.Name);
            var roles = await userManager.GetRolesAsync(re);

            return View();
        }
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string rolename)
        {
            bool x = await RoleManager.RoleExistsAsync(rolename);
            if (!x)
            {
                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = rolename;
                await RoleManager.CreateAsync(role);
                ViewBag.rol = "Successfully Created role";
                //Here we create a Admin super user who will maintain the website                   


            }
            else
            {
                ViewBag.rol = "Already Exist";
            }
            return View();
        }
      
    }
}
