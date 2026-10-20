using CourseEnrollment.Extensions;
using CourseEnrollment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CourseEnrollment.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IMemoryCache _cache;

        public CoursesController(IMemoryCache cache)
        {
            _cache = cache;
        }

        private List<Course> GetCachedCourses()
        {
            return _cache.GetOrCreate("CourseList", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(5);

                return new List<Course>
                {
                    new Course
                    {
                        CourseId = 1,
                        CourseName = "ASP.NET Core MVC",
                        Duration = "2 Months",
                        Fees = 10000m,
                        DiscountPercentage = 20m
                    },
                    new Course
                    {
                        CourseId = 2,
                        CourseName = "Python Programming",
                        Duration = "3 Months",
                        Fees = 12000m,
                        DiscountPercentage = 10m
                    },
                    new Course
                    {
                        CourseId = 3,
                        CourseName = "Full Stack Development",
                        Duration = "4 Months",
                        Fees = 18000m,
                        DiscountPercentage = 15m
                    },
                    new Course
                    {
                        CourseId = 4,
                        CourseName = "Machine Learning",
                        Duration = "3 Months",
                        Fees = 15000m,
                        DiscountPercentage = 12m
                    }
                };
            })!;
        }

        public IActionResult Index()
        {
            ViewBag.StudentName =
                Request.Cookies["StudentName"];

            return View(GetCachedCourses());
        }

        [ResponseCache(
            Duration = 60,
            Location = ResponseCacheLocation.Any,
            NoStore = false)]
        public IActionResult Details(int id)
        {
            var course = GetCachedCourses()
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpGet]
        public IActionResult Enroll(int id)
        {
            var course = GetCachedCourses()
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            HttpContext.Session.SetInt32("SelectedCourse", id);

            ViewBag.StudentName = Request.Cookies["StudentName"];

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(int courseId, string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentName) ||
                studentName.Length > 100)
            {
                ModelState.AddModelError(
                    "studentName",
                    "Enter a name of up to 100 characters.");

                var invalidCourse = GetCachedCourses()
                    .FirstOrDefault(c => c.CourseId == courseId);

                if (invalidCourse == null)
                    return NotFound();

                return View(invalidCourse);
            }

            var course = GetCachedCourses()
                .FirstOrDefault(c => c.CourseId == courseId);

            if (course == null)
                return NotFound();

            Response.Cookies.Append(
                "StudentName",
                studentName.Trim(),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                });

            HttpContext.Session.SetInt32("SelectedCourse", courseId);

            TempData["SuccessMessage"] =
                $"Enrollment successful for {studentName.Trim()}! " +
                $"Course: {course.CourseName}. " +
                $"Final Fees: ₹{course.CalculateFinalFees():N2}";

            return RedirectToAction(nameof(Confirmation));
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}