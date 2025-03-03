using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
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
                    // Handle the case where the cookie is missing.  You have several options:
                    // 1. Redirect (as you're doing now)
                    // return RedirectToPage("/Account/Login", new { area = "Identity" });  // This won't work directly in a helper

                    // 2. Throw an exception (more testable)
                    throw new InvalidOperationException($"Missing authentication cookie: {authCookieName}");

                    // 3. Return null (and handle the null in the calling method)
                    // return null;

                    // 4. Return an HttpClient that might work for some requests (e.g. public API)
                    // return _httpClientFactory.CreateClient(); // Or create a specific client
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
                var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
                    var httpClient = GetAuthenticatedHttpClient();

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


    }
}
