using Microsoft.AspNetCore.Mvc;
using NotesApplication.Interfaces;
using NotesDomain;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Session;
using NotesApplication.Interface;
using NotesApplication.Services;
using DTOs;
using PagedList.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
namespace NotesApp.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }


       
        //public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        //{
        //    try
        //    {
        //        if (HttpContext.Session.GetString("UserId") == null)
        //        {
        //            return RedirectToAction("Login", "User");
        //        }

        //        // Set up sorting parameters
        //        ViewData["CurrentSort"] = sortOrder;
        //        ViewData["CategorySortParm"] = sortOrder == "category" ? "category_desc" : "category";
        //        ViewData["TitleSortParm"] = sortOrder == "title" ? "title_desc" : "title";
        //        ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
        //        ViewData["TagsSortParm"] = sortOrder == "tags" ? "tags_desc" : "tags";

        //        // Handle search parameters
        //        if (searchString != null)
        //        {
        //            pageNumber = 1; // Reset to first page with new search
        //        }
        //        else
        //        {
        //            searchString = currentFilter;
        //        }

        //        ViewData["CurrentFilter"] = searchString;

        //        // Get user ID from session
        //        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

        //        // Get notes from service as an IQueryable
        //        var notesQuery = await _noteService.GetNotesByUserIdAsync(userId);

        //        // Apply search filters on the query
        //        if (!string.IsNullOrEmpty(searchString))
        //        {
        //            notesQuery = notesQuery.Where(n =>
        //                n.Title.Contains(searchString) ||
        //                (n.Content != null && n.Content.Contains(searchString)) ||
        //                (n.Category != null && n.Category.Contains(searchString)) ||
        //                (n.Tags != null && n.Tags.Contains(searchString))
        //            );
        //        }

        //        // Apply sorting on the query
        //        notesQuery = ApplySorting(notesQuery, sortOrder);

        //        // Set page size
        //        int pageSize = 3;

        //        // Get total count for pagination
        //        int totalCount = notesQuery.Count();

        //        // Apply pagination
        //        var notes = notesQuery
        //            .Skip(((pageNumber ?? 1) - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        // Create the paginated list
        //        var paginatedNotes = new PaginatedList<NoteDTO>(
        //            notes,
        //            totalCount,
        //            pageNumber ?? 1,
        //            pageSize);

        //        return View(paginatedNotes);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Consider logging the exception
        //        ModelState.AddModelError("", "An error occurred while retrieving notes.");
        //        return View(new PaginatedList<NoteDTO>(new List<NoteDTO>(), 0, 1, 1));
        //    }
        //}

        
        //private IQueryable<NoteDTO> ApplySorting(IQueryable<NoteDTO> notes, string sortOrder)
        //{
        //    switch (sortOrder)
        //    {
        //        case "title_desc":
        //            return notes.OrderByDescending(n => n.Title);
        //        case "title":
        //            return notes.OrderBy(n => n.Title);
        //        case "category_desc":
        //            return notes.OrderByDescending(n => n.Category);
        //        case "category":
        //            return notes.OrderBy(n => n.Category);
        //        case "date_desc":
        //            return notes.OrderByDescending(n => n.CreatedDate);
        //        case "date":
        //            return notes.OrderBy(n => n.CreatedDate);
        //        case "tags_desc":
        //            return notes.OrderByDescending(n => n.Tags);
        //        case "tags":
        //            return notes.OrderBy(n => n.Tags);
        //        default:
        //            return notes.OrderByDescending(n => n.CreatedDate); // Default sort by newest
        //    }
        //}

        // AJAX Search Method with server-side pagination
        //public async Task<JsonResult> Search(string searchTerm, int? page = 1)
        //{
        //    try
        //    {
        //        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        //        int pageSize = 3;

        //        // Get notes from service as an IQueryable
        //        var notesQuery = await _noteService.GetNotesByUserIdAsync(userId);

        //        // Apply search filter if provided
        //        if (!string.IsNullOrEmpty(searchTerm))
        //        {
        //            notesQuery = notesQuery.Where(n =>
        //                n.Title.Contains(searchTerm) ||
        //                (n.Content != null && n.Content.Contains(searchTerm)) ||
        //                (n.Category != null && n.Category.Contains(searchTerm)) ||
        //                (n.Tags != null && n.Tags.Contains(searchTerm))
        //            );
        //        }

        //        // Order by created date (default sort for AJAX search)
        //        notesQuery = notesQuery.OrderByDescending(n => n.CreatedDate);

        //        // Get total count for pagination
        //        int totalCount = notesQuery.Count();
        //        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        //        // Apply pagination
        //        var notes = notesQuery
        //            .Skip(((page ?? 1) - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(n => new
        //            {
        //                id = n.Id,
        //                title = n.Title,
        //                content = n.Content,
        //                category = n.Category,
        //                tags = n.Tags,
        //                createdDate = n.CreatedDate
        //            })
        //            .ToList();

        //        // Return paginated results with metadata
        //        return Json(new
        //        {
        //            notes = notes,
        //            currentPage = page ?? 1,
        //            totalPages = totalPages,
        //            totalCount = totalCount,
        //            hasNextPage = (page ?? 1) < totalPages,
        //            hasPreviousPage = (page ?? 1) > 1
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = "An error occurred while searching notes." });
        //    }
        //}

        public async Task<IActionResult> Search(string searchTerm)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var notes = await _noteService.GetNotesByUserIdAsync(userId);
            if (!string.IsNullOrEmpty(searchTerm))
                {
                   notes = notes.Where(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                         || n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (notes is null)
            {
                return NotFound();
            }
            return Ok(notes);


        }



        //public async Task<JsonResult> Getdata(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        //{
        //    ViewData["CurrentSort"] = sortOrder;

        //    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        //    var notes = await _noteService.GetNotesByUserIdAsync(userId);

        //    if (searchString != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    int pageSize = 3;

            
        //    var pagedNotes = PaginatedList<NoteDTO>.CreateAsync(notes, pageNumber ?? 1, pageSize);

        //    return Json(pagedNotes); // Use Json() helper method
        //}

        //public async Task<IActionResult> Index(int? pageNumber)
        //{
        //    try
        //    {

        //        //if (HttpContext.Session.GetString("UserId") == null)
        //        //{
        //        //    return RedirectToAction("Login", "User");
        //        //}

        //        //ViewData["CurrentSort"] = sortOrder;

        //        ////int userId = int.Parse(HttpContext.Session.GetString("UserId"));
        //        //int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        //        //var notes = await _noteService.GetNotesByUserIdAsync(userId);
        //        ////if (!string.IsNullOrEmpty(searchTerm))
        //        ////{
        //        ////    notes = notes.Where(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        //        ////                          || n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        //        ////}
        //        //if (searchString != null)
        //        //{
        //        //    pageNumber = 1;
        //        //}
        //        //else
        //        //{
        //        //    searchString = currentFilter;
        //        //}
        //        //int pageSize = 3;
        //        //return View(PaginatedList<NoteDTO>.CreateAsync(notes, pageNumber ?? 1, pageSize));
        //        return View();

        //        //return View(notes);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //ye iss ko hum debug krein ge phly jb create khule gaa tu get wala aur jb um create k button pr click krein ge 
        //post wala actionmethod aaa jei ga aur jb hum debug krein ge tu ye iss tarh hoga k post wale k andar info aaa jei ge notes ki
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddUpdateNoteDTO note)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            note.UserId = HttpContext.Session.GetInt32("UserId");
            note.CreatedDate = DateTime.Now;
            await _noteService.AddNoteAsync(note);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("User", "Login");
            }
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null || note.UserId != HttpContext.Session.GetInt32("UserId"))
            {
                return RedirectToAction("Index");
            }
            return View(note);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AddUpdateNoteDTO note)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            note.UserId = HttpContext.Session.GetInt32("UserId");
            note.CreatedDate = DateTime.Now;
            await _noteService.UpdateNoteAsync(note);
            // TempData["message"] = "updated successfully YAhooooo!";
            return RedirectToAction("Index");

        }
        //[HttpGet]
        //public async Task<IActionResult> DeleteConfirm(int id)
        //{
        //    if (HttpContext.Session.GetInt32("UserId") == null)
        //    {
        //        return RedirectToAction("User", "Login");
        //    }
        //    var note = await _noteService.GetNoteByIdAsync(id);
        //    if (note == null || note.UserId != HttpContext.Session.GetInt32("UserId"))
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    return View(note);
        //}



        //[HttpGet]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    if (HttpContext.Session.GetInt32("UserId") == null)
        //    {
        //        return RedirectToAction("Login", "User");
        //    }

        //    var note = await _noteService.GetNoteByIdAsync(id);
        //    if (note == null || note.UserId != HttpContext.Session.GetInt32("UserId"))
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    await _noteService.DeleteNoteAsync(note);
        //    TempData["message"] = "Deleted successfully!";
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null || note.UserId != userId)
                return NotFound();

            await _noteService.DeleteNoteAsync(note);
            return Ok();
        }
        //public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        //{
        //    try
        //    {

        //        if (HttpContext.Session.GetString("UserId") == null)
        //        {
        //            return RedirectToAction("Login", "User");
        //        }

        //        ViewData["CurrentSort"] = sortOrder;

        //        //int userId = int.Parse(HttpContext.Session.GetString("UserId"));
        //        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        //        var notes = await _noteService.GetNotesByUserIdAsync(userId);
        //        //if (!string.IsNullOrEmpty(searchTerm))
        //        //{
        //        //    notes = notes.Where(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        //        //                          || n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        //        //}
        //        if (searchString != null)
        //        {
        //            pageNumber = 1;
        //        }
        //        else
        //        {
        //            searchString = currentFilter;
        //        }
        //        int pageSize = 3;
        //        var test = PaginatedList<NoteDTO>.CreateAsync(notes, pageNumber ?? 1, pageSize);
        //        return View(PaginatedList<NoteDTO>.CreateAsync(notes, pageNumber ?? 1, pageSize));

        //        //return View(notes);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //[HttpGet]
        //public JsonResult getAllNotes(string txtSearch, int? page)
        //{
        //    var data = (from s in _noteService.Notes select s);
        //    if (!String.IsNullOrEmpty(txtSearch))
        //    {
        //        ViewBag.txtSearch = txtSearch;
        //        data = data.Where(s => s.Title.Contains(txtSearch));
        //    }
        //    if (page > 0)
        //    {
        //        page = page;
        //    }
        //    else
        //    {
        //        page = 1;
        //    }
        //    int start = (int)(page - 1) * pageSize;
        //    ViewBag.pageCurrent = page;
        //    int totalPage = data.Count();
        //    float totalNumsize = (totalPage / (float)pageSize);
        //    int numSize = (int)Math.Ceiling(totalNumsize);
        //    ViewBag.numSize = numSize;
        //    var dataPost = data.OrderByDescending(x => x.Id).Skip(start).Take(pageSize);
        //    List<Note> listPost = new List<Note>();
        //    listPost = dataPost.ToList();
        //    // return Json(listPost);
        //    return Json(new { data = listPost, pageCurrent = page, numSize = numSize }, JsonRequestBehavior.AllowGet);
        //}

        public async Task<IActionResult> Index(string searchTerm, string sortOrder, int? pageNumber)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var notes = await _noteService.GetNotesByUserIdAsync(userId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                notes = notes.Where(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sort (basic example)
            switch (sortOrder)
            {
                case "Title":
                    notes = notes.OrderBy(n => n.Title).ToList();
                    break;
                case "Category":
                    notes = notes.OrderBy(n => n.Category).ToList();
                    break;
                case "Tags":
                    notes = notes.OrderBy(n => n.Tags ?? "").ToList();
                    break;
                case "CreatedDate":
                    notes = notes.OrderBy(n => n.CreatedDate).ToList();
                    break;
            }

            int pageSize = 5;
            var paginatedList = PaginatedList<NoteDTO>.CreateAsync(notes.AsQueryable(), pageNumber ?? 1, pageSize);

            // If AJAX call, return partial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_NotesPartial", paginatedList);
            }

            // Else return full view
            return View(paginatedList);
        }





    }

}
