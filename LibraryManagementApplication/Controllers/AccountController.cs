using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace LibraryManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory
            _httpClientFactory;

        public AccountController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory =
                httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client =
                _httpClientFactory
                    .CreateClient("LibraryAPI");

            var response =
                await client.PostAsJsonAsync(
                    "api/Auth/register",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Registration successful. Please login.";

                return RedirectToAction("Login");
            }

            ModelState.AddModelError(
                "",
                "Registration failed. Email may already exist.");

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client =
                _httpClientFactory
                    .CreateClient("LibraryAPI");

            var response =
                await client.PostAsJsonAsync(
                    "api/Auth/login",
                    model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<LoginResponse>();

            if (result == null ||
                string.IsNullOrWhiteSpace(
                    result.Token))
            {
                ModelState.AddModelError(
                    "",
                    "Login failed.");

                return View(model);
            }

            HttpContext.Session.SetString(
                "JWToken",
                result.Token);

            HttpContext.Session.SetString(
                "UserId",
                result.UserId.ToString());

            HttpContext.Session.SetString(
                "UserName",
                result.Name);

            HttpContext.Session.SetString(
                "Role",
                result.Role);

            return RedirectToAction(
                "Index",
                "Books");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login");
        }
    }
}