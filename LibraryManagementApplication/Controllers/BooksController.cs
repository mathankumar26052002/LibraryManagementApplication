using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LibraryManagementMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory
            _httpClientFactory;

        public BooksController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory =
                httpClientFactory;
        }

        private HttpClient GetClient()
        {
            var client =
                _httpClientFactory
                    .CreateClient("LibraryAPI");

            var token =
                HttpContext.Session
                    .GetString("JWToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }

            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string search = null)
        {
            var client = GetClient();

            var url = "api/Books";

            if (!string.IsNullOrWhiteSpace(search))
            {
                url +=
                    "?search=" +
                    Uri.EscapeDataString(search);
            }

            var response =
                await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to load books.";

                return View(
                    new List<BookViewModel>());
            }

            var books =
                await response.Content
                    .ReadFromJsonAsync<
                        List<BookViewModel>>();

            ViewBag.Search = search;

            return View(
                books ??
                new List<BookViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session
                    .GetString("Role") != "Admin")
            {
                return Forbid();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            BookViewModel model)
        {
            if (HttpContext.Session
                    .GetString("Role") != "Admin")
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = GetClient();

            var response =
                await client.PostAsJsonAsync(
                    "api/Books",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Book added successfully.";

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(
                "",
                "Unable to add book.");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
        {
            if (HttpContext.Session
                    .GetString("Role") != "Admin")
            {
                return Forbid();
            }

            var client = GetClient();

            var response =
                await client.GetAsync(
                    $"api/Books/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var book =
                await response.Content
                    .ReadFromJsonAsync<
                        BookViewModel>();

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            BookViewModel model)
        {
            if (HttpContext.Session
                    .GetString("Role") != "Admin")
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = GetClient();

            var response =
                await client.PutAsJsonAsync(
                    $"api/Books/{model.Id}",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Book updated successfully.";

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(
                "",
                "Unable to update book.");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(
            int id)
        {
            if (HttpContext.Session
                    .GetString("Role") != "Admin")
            {
                return Forbid();
            }

            var client = GetClient();

            var response =
                await client.DeleteAsync(
                    $"api/Books/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Book deleted successfully.";
            }
            else
            {
                TempData["Error"] =
                    "Unable to delete book.";
            }

            return RedirectToAction("Index");
        }
    }
}