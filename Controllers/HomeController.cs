namespace StrategoGameServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("/")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMap()
        {
            var mapContent = System.IO.File.ReadAllLines("./wwwroot/map.html").Aggregate((a, b) => a + "\n" + b);
            return Content(mapContent, "text/html");
        }
    }
}