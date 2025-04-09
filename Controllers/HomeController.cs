namespace StrategoGameServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMap()
        {
            var mapContent = System.IO.File.ReadAllLines("map.html").Aggregate((a, b) => a + "\n" + b);
            return Content(mapContent, "text/html");
        }
    }
}