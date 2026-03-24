using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();
        public IActionResult HowItWorks() => View();
        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult FAQ() => View();
        public IActionResult Insurance() => View();
        public IActionResult Safety() => View();
        public IActionResult Terms() => View();
        public IActionResult Privacy() => View();
    }
}
