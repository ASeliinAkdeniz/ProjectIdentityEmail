using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectIdentityEmail.Dtos;
using ProjectIdentityEmail.Entities;

namespace ProjectIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        public LoginController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult UserLogin() // Index ismini UserLogin yaptın
        {
            return View(); // Artık otomatik olarak Views/Login/UserLogin.cshtml dosyasını bulur
        }
        [HttpPost]
        public async Task<ActionResult> UserLogin(LoginUserDto loginUserDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUserDto.UserName, loginUserDto.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya parola hatalı");
            }
            return View();
        }
    }
}
