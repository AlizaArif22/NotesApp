using Microsoft.AspNetCore.Mvc;
using NotesApplication.Interfaces;
using NotesDomain;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NotesMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

       
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {   
                ModelState.AddModelError("", "Email and Password are required!");
                return View();
            }
            
            var Authenticated = await _userService.AuthenticateUserAsync(email, password);
            if (Authenticated)
            {
                var user = await _userService.GetUserByEmailAsync(email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);
               

                if (user.Email == "admin@gmail.com" && user.Username == "Admin")
                {
                    HttpContext.Session.SetString("IsAdmin", "aliza");
                    TempData["SuccessMessage"] = "Welcome, Admin!";
                    return RedirectToAction("TableBasic", "Dashboard");
                }
                else
                {
                    HttpContext.Session.SetString("IsAdmin", "false");
                }
                //HttpContext.Session.SetString("IsAdmin", "false");
                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password!");
            return View();
        }

        
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                
                var existingUser = await _userService.GetUserByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(user);
                }

                
                if (user.Email == "alizziiarif@gmail.com" && user.Username == "Aliza Arif")
                {
                    user.IsAdmin = true;
                }

                

                await _userService.RegisterUserAsync(user);
                TempData["SuccessMessage"] = "Registration successful! Yahoooooo ";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.Message, "ERROR:)");
                return View(user);
            }
        }


        public IActionResult LogoutConfirmation()
        {
            if (HttpContext.Session.Get("UserId") == null)
            { 
            return RedirectToAction("Login");
            }
            return View();
        }

        
        [HttpPost]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.Get("UserId") == null)
                return RedirectToAction("Login");

            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _userService.GetUserByIdAsync(userId);

            
            //string userRole = HttpContext.Session.GetString("UserRole") ?? "User";
            //ViewBag.Role = userRole;

            return View(user);
        }
 
        public async Task<IActionResult> Index()
        {
            // var users = await _userService.GetAllUsersAsync();
            // return View(users);
            return View();
        }
    }
}
