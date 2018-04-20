using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Activities.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Activities
{
    public class LoginController : Controller
    {
        private ActivitiesContext _context;
        public LoginController(ActivitiesContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User newUser = new User{
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password
                };
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                int userId = _context.users.Single( u => (string)u.Email == (string)model.Email).UserId;
                HttpContext.Session.SetInt32("activeUser", userId);
                return RedirectToAction("Clean", "Activity");
            }
            return View("Index");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string Email, string Password)
        {
            List<User> possibleLogin = _context.users.Where( u => (string)u.Email == (string)Email).ToList();
            if(possibleLogin.Count == 1)
            {
                var Hasher = new PasswordHasher<User>();
                if(0!= Hasher.VerifyHashedPassword(possibleLogin[0], possibleLogin[0].Password, Password))
                {
                    HttpContext.Session.SetInt32("activeUser", possibleLogin[0].UserId);
                    return RedirectToAction("Clean", "Activity");
                }
            }
            ViewBag.error = "Incorrect Login Information";
            return View("Index");
        }
    }
}