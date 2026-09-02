using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace LibraryManagementMVC.Controllers
{
    public class BorrowController : Controller
    {
        private readonly IHttpClientFactory
            _httpClientFactory;

        public BorrowController(
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

        [HttpPost]
        public async Task<IActionResult> Borrow(
            int bookId)
        {
            if (string.IsNullOrWhiteSpace(
                HttpContext.Session
                    .GetString("JWToken")))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var client = GetClient();

            var response =
                await client.PostAsync(
                    $"api/Borrow/{bookId}",
                    null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Book borrowed successfully.";
            }
            else
            {
                TempData["Error"] =
                    "Unable to borrow book.";
            }

            return RedirectToAction(
                "Index",
                "Books");
        }

        [HttpPost]
        public async Task<IActionResult> Return(
            int bookId)
        {
            if (string.IsNullOrWhiteSpace(
                HttpContext.Session
                    .GetString("JWToken")))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var client = GetClient();

            var response =
                await client.PostAsync(
                    $"api/Borrow/return/{bookId}",
                    null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Book returned successfully.";
            }
            else
            {
                TempData["Error"] =
                    "Unable to return book.";
            }

            return RedirectToAction(
                "Index",
                "Books");
        }
    }
}