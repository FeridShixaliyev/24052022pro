using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaSyte.Helpers;
using ProniaSyte.Models;
using ProniaSyte.ViewModels.Authorization;
using System;
using System.Threading.Tasks;

namespace ProniaSyte.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            if (register == null) return BadRequest();
            AppUser newUser = new AppUser
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                UserName = register.Username,
                Email = register.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, register.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return View();
                }
            }
            await _userManager.AddToRoleAsync(newUser, UserRoles.Member.ToString());
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string ReturnUrl)
        {
            AppUser user;
            if (loginVM.UsernameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            }
            if (user == null)
            {
                ModelState.AddModelError("", "Sifre,Email ve ya Istifadeci adi yanlisdir");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Daxil olma limitin ashdiz,Zehmet olmasa biraz gozleyin...");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Sifre,Email ve ya Istifadeci adi yanlisdir");
                return View();
            }
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Admin"))
            {
                return RedirectToAction("Index","Dashboard",new { area="Manage"});
            }
            if (ReturnUrl != null) return LocalRedirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }
        public async Task CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(item.ToString()));
                }
            }
        }
    }
}
