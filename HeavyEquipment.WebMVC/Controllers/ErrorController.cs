using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            return statusCode switch
            {
                404 => View("NotFound"),
                _ => View("Error")
            };
        }

        [Route("Error")]
        public IActionResult HandleError()
        {
            return View("Error");
        }
    }
}
