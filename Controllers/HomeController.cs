using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LoginAndReg.Models;

namespace LoginAndReg.Controllers
{
    public class HomeController : Controller
    {
        private LoginContext dbContext;
        public HomeController(LoginContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            int? loggedID = HttpContext.Session.GetInt32("LoggedId");
            if (loggedID == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Success");
            }
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    newUser.Password = hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    User justMade = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                    HttpContext.Session.SetInt32("LoggedId", justMade.UserId);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View ("Index");
            }
        }

        [HttpGet("login")]
        public IActionResult LoginForm()
        {
            int? loggedID = HttpContext.Session.GetInt32("LoggedId");
            if (loggedID == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Success");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userLogin.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("LoginForm");
                }
                else
                {
                    PasswordHasher<UserLogin> hasher = new PasswordHasher<UserLogin>();
                    PasswordVerificationResult check = hasher.VerifyHashedPassword(userLogin, userInDb.Password, userLogin.Password);
                    if (check == 0)
                    {
                        ModelState.AddModelError("Email", "Invalid Email/Password");
                        return View("LoginForm");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("LoggedId", userInDb.UserId);
                        return RedirectToAction("Success");
                    }
                }
            }
            else
            {
                return View("LoginForm");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            int? loggedID = HttpContext.Session.GetInt32("LoggedId");
            if (loggedID == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
