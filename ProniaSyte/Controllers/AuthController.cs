using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaSyte.Models;
using ProniaSyte.ViewModels.Authorization;
using System.Threading.Tasks;

namespace ProniaSyte.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                FirstName=register.FirstName,
                LastName=register.LastName,
                UserName=register.Username,
                Email=register.Email,
            };
            IdentityResult result= await _userManager.CreateAsync(newUser,register.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return View();
                }
            }
            await _signInManager.SignInAsync(newUser,true);
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
