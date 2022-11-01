using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShibbolethApp.Identity;
using ShibbolethApp.Models;

namespace ShibbolethApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Authorize()
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Student)]
        public IActionResult Student()
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Employee)]
        public IActionResult Employee()
        {
            return View();
        }

        [Authorize(Roles = "employee, student")]
        public IActionResult StudentOrEmployee()
        {
            return View();
        }

        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
