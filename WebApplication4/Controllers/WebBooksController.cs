using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class WebBooksController : Controller
    {

            private readonly string _apiBaseUrl = "https://localhost:7237"; // Or "http://localhost:5124"
            private readonly string authCookieName = ".AspNetCore.Identity.Application";
            private readonly IHttpClientFactory _httpClientFactory;

            public WebBooksController(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            // GET: WebBooksController
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var httpClient = _httpClientFactory.CreateClient(); 
                var authCookieValue = Request.Cookies[authCookieName]; 

                if (authCookieValue != null)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                }
                else
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(books);

            }

            // GET: WebBooksController/Details/5
            [HttpGet]
            public async Task<IActionResult> Details(int id)
            {   
                var httpClient = _httpClientFactory.CreateClient(); // And create here
                var authCookieValue = Request.Cookies[authCookieName]; 

                if (authCookieValue != null)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                }
                else
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books/{id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(book);

            }



            // GET: WebBooks/Create
            [HttpGet]
            public IActionResult Create()
                {
                    return View(); // Display the create form
                }

            // POST: WebBooks/Create
            [HttpPost]
            [ValidateAntiForgeryToken] // Prevents cross-site request forgery
            public async Task<IActionResult> Create(Book book)
            {
                if (ModelState.IsValid)
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var authCookieValue = Request.Cookies[authCookieName]; 

                    if (authCookieValue != null)
                    {
                        httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                    }
                    else
                    {
                        return RedirectToPage("/Account/Login", new { area = "Identity" });
                    }

                    var json = JsonSerializer.Serialize(book);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{_apiBaseUrl}/api/books", content);
                    response.EnsureSuccessStatusCode();

                    // Optionally, handle the response (e.g., redirect to details)
                    var createdBookJson = await response.Content.ReadAsStringAsync();
                    var createdBook = JsonSerializer.Deserialize<Book>(createdBookJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return RedirectToAction(nameof(Index)); // Redirect to the list of books
                }
                return View(book); // Return the view with validation errors
            }


            // GET: WebBooksController/Edit/5
            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var httpClient = _httpClientFactory.CreateClient();
                var authCookieValue = Request.Cookies[authCookieName]; 

                if (authCookieValue != null)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                }
                else
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(); // Or handle the error appropriately
                }
                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, Book book)
            {
                if (id != book.Id)
                {
                    return BadRequest();
                }

                if (ModelState.IsValid)
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var authCookieValue = Request.Cookies[authCookieName]; 

                    if (authCookieValue != null)
                    {
                        httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                    }
                    else
                    {
                        return RedirectToPage("/Account/Login", new { area = "Identity" });
                    }
                    var json = JsonSerializer.Serialize(book);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync($"{_apiBaseUrl}/api/books", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index)); // Redirect to the list
                }
                return View(book); // Return the view with validation errors
            }



            // GET: WebBooksController/Delete/5

            public async Task<IActionResult> Delete(int id)
            {
                var httpClient = _httpClientFactory.CreateClient();
                var authCookieValue = Request.Cookies[authCookieName]; 

                if (authCookieValue != null)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                }
                else
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(); // Or handle the error appropriately
                }
                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (book == null)
                {
                    return NotFound();
                }
                return View(book);

            }

            // POST: WebBooksController/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var httpClient = _httpClientFactory.CreateClient();
                var authCookieValue = Request.Cookies[authCookieName]; 

                if (authCookieValue != null)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}"); 
                }
                else
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var response = await httpClient.DeleteAsync($"{_apiBaseUrl}/api/books/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(); // Or handle the error appropriately
                }

                return RedirectToAction(nameof(Index));
            }















        //    private readonly ApplicationDbContext _context;

        //    public WebBooksController(ApplicationDbContext context)
        //    {
        //        _context = context;
        //    }

        //    // GET: WebBooks
        //    public async Task<IActionResult> Index()
        //    {
        //        return View(await _context.Books.ToListAsync());
        //    }

        //    // GET: WebBooks/Details/5
        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var book = await _context.Books
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (book == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(book);
        //    }

        //    // GET: WebBooks/Create
        //    public IActionResult Create()
        //    {
        //        return View();
        //    }

        //    // POST: WebBooks/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("Id,Title")] Book book)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(book);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(book);
        //    }

        //    // GET: WebBooks/Edit/5
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var book = await _context.Books.FindAsync(id);
        //        if (book == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(book);
        //    }

        //    // POST: WebBooks/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] Book book)
        //    {
        //        if (id != book.Id)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(book);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!BookExists(book.Id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(book);
        //    }

        //    // GET: WebBooks/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var book = await _context.Books
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (book == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(book);
        //    }

        //    // POST: WebBooks/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var book = await _context.Books.FindAsync(id);
        //        if (book != null)
        //        {
        //            _context.Books.Remove(book);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool BookExists(int id)
        //    {
        //        return _context.Books.Any(e => e.Id == id);
        //    }
    }
}
