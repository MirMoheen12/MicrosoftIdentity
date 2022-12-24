using BrinRose.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrinRose.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
      public class ModeratorsideController : Controller
    {
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> RoleManager;
        public ModeratorsideController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            RoleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllRoles()
        {
            List<string> roles = RoleManager.Roles.Select(x => x.Name).ToList();
            List<AllUsersRoles> userlist = new List<AllUsersRoles>();
            for (int i = 0; i < roles.Count; i++)
            {
                AllUsersRoles allUsersRoles = new AllUsersRoles();
                allUsersRoles.Rid = i+1;
                allUsersRoles.Rname=roles[i];
                userlist.Add(allUsersRoles);

            }

            return View(userlist);
        }
    }
}
