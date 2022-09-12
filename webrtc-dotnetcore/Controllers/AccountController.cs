using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using webrtc_dotnetcore.Model.Account;
using webrtc_dotnetcore.Services;

namespace webrtc_dotnetcore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAccount _userAccount;
        public AccountController(IUserAccount userAccount)
        {
            _userAccount = userAccount;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Login Failed";
                return View(model);
            }
            User user = await _userAccount.Login(model);
            if (user.Id > 0)
            {
                await Authenticate(user);
                return RedirectToAction("Agent", "Micred");
            }
            else
            {
                ViewBag.Error = "Login Failed";
                return View();
            }
        }

        public async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId",user.Id.ToString()),
                new Claim("Username",user.UserName),
                new Claim("OrganizationId",user.OrganizationId.ToString())
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
