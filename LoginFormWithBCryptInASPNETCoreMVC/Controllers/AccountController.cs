using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginFormWithBCryptInASPNETCoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginFormWithBCryptInASPNETCoreMVC.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private AppDbContext db = null;
        public AccountController(AppDbContext db)
        {
            this.db = db;
        }


        [Route("")]
        [Route("index")]
        [Route("~/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("signup")]
        public IActionResult SignUp()
        {
            return View("SignUp", new Account());
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult SignUp(Account account)
        {
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            db.Account.Add(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string username,string password)
        {
            var account = checkAccount(username, password);
            if (account == null)
            {
                ViewBag.error = "Invalid";
                return View("Index");
            } else
            {
                HttpContext.Session.SetString("username", username);
                return View("Success");
            }
            //return RedirectToAction("Index");
        }

        private Account checkAccount(string username, string password)
        {
            var account = db.Account.SingleOrDefault(a => a.Username.Equals(username));
            if (account != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password,account.Password))
                {
                    return account;
                }
            }
            return null;
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }
    }
}