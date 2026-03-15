using Microsoft.AspNetCore.Mvc;
using NotesApplication.Interface;
using NotesApplication.Interfaces;
using NotesDomain;

namespace NotesApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUserService _userService;
        private readonly INoteService _noteService;
        public DashboardController(INoteService noteService, IUserService userService)
        {
            _userService = userService;
            _noteService = noteService; 
        }
        public  async Task <IActionResult>  TableBasic([FromForm]User user)
        {

            //if (HttpContext.Session.Get("UserId") == null)
            //{
            //    return RedirectToAction("Login","User");
            //}
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            
             if (!(isAdmin == "aliza"))
            {
                return RedirectToAction("Login", "User");
            }
            var users = await _userService.GetAllUsersAsync();
             return View(users);
        }

        //public async Task<IActionResult> ViewUsers()
        //{
        //    if ()
        //    var users = await _userService.GetAllUsersAsync();
        //    return View(users);
        //}
    }
}
