using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebApplication4.Dtos;
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

            private HttpClient GetAuthenticatedHttpClient()
            {
                var httpClient = _httpClientFactory.CreateClient();
                var authCookieValue = Request.Cookies[authCookieName];

                if (authCookieValue == null)
                {
                    throw new InvalidOperationException($"Missing authentication cookie: {authCookieName}");

                }

                httpClient.DefaultRequestHeaders.Add("Cookie", $"{authCookieName}={authCookieValue}");
                return httpClient;
            }

            // GET: WebBooksController
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var httpClient = GetAuthenticatedHttpClient();

                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<List<BookDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(books);

            }

            // GET: WebBooksController/Details/5
            [HttpGet]
            public async Task<IActionResult> Details(int id)
            {   
                var httpClient = GetAuthenticatedHttpClient();

                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books/{id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<BookDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(book);

            }



            // GET: WebBooks/Create
            [HttpGet]
            public async Task<IActionResult> Create()
                {
                    var HttpClient = GetAuthenticatedHttpClient();
                    var response = await HttpClient.GetAsync($"{_apiBaseUrl}/api/books/create");
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var book = JsonSerializer.Deserialize<BookCreateDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(book); // Display the create form
                }

            // POST: WebBooks/Create
            [HttpPost]
            [ValidateAntiForgeryToken] // Prevents cross-site request forgery
            public async Task<IActionResult> Create(BookCreateDto book)
            {
                if (ModelState.IsValid)
                {
                    var httpClient = GetAuthenticatedHttpClient();

                    var json = JsonSerializer.Serialize(book);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{_apiBaseUrl}/api/books", content);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index)); // Redirect to the list of books
                }
                return View(book); // Return the view with validation errors
            }


            // GET: WebBooksController/Edit/5
            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var httpClient = GetAuthenticatedHttpClient();

                var response = await httpClient.GetAsync($"{_apiBaseUrl}/api/books/edit/{id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<BookEditDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, BookEditDto book)
            {
                if (id != book.Id)
                {
                    return BadRequest();
                }

                if (ModelState.IsValid)
                {
                    var httpClient = GetAuthenticatedHttpClient();

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
                var httpClient = GetAuthenticatedHttpClient();

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
                var httpClient = GetAuthenticatedHttpClient();

                var response = await httpClient.DeleteAsync($"{_apiBaseUrl}/api/books/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(); // Or handle the error appropriately
                }

                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            public async Task<IActionResult> Search(string? searchString = null , int? numberOfPages = null ,DateOnly? publishDate = null, string? author=null) {

                var HttpClient = GetAuthenticatedHttpClient();
                var response = await HttpClient.GetAsync($"{_apiBaseUrl}/api/books?searchstring={searchString}&numberofpages={numberOfPages}&publishdate={publishDate:yyyy-MM-dd}&author={author}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<List<BookDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); 

                return View(books); 
            }


    }
}
