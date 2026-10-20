using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollment.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var users = await client.GetFromJsonAsync<List<ApiUser>>(
                    "https://jsonplaceholder.typicode.com/users");

                return View(users ?? new List<ApiUser>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error =
                    "Unable to retrieve user information. Please try again.";

                return View(new List<ApiUser>());
            }
            catch (TaskCanceledException)
            {
                ViewBag.Error =
                    "The request timed out. Please try again.";

                return View(new List<ApiUser>());
            }
        }
    }

    public class ApiUser
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public CompanyInfo? Company { get; set; }
    }

    public class CompanyInfo
    {
        public string Name { get; set; } = string.Empty;
    }
}