using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetFlow.Portal.Model;
using TweetFlow.Services;

namespace TweetFlow.Portal.Controllers
{
    public class AccountController : Controller
    {
        private AccountService accountService;
        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(AdminController.Index), "Admin");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(HomeController.Index), "Home", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }

        [HttpPost]
        [Route("/api/account/signup")]
        public IActionResult SignUp([FromBody] SignupModel model)
        {
            if(model == null)
            {
                model = new SignupModel();
            }

            var result = this.accountService.SaveEmail(model.Email);
            return Ok(new {
                result = result
            });
        }
    }
}
