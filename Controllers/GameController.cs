namespace StrategoGameServer.Controllers
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using StrategoGameServer.Records;

    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private static readonly List<Game> Games = [];

        [HttpGet("getGames")]
        public IActionResult GetGames()
        {
            return Ok(Games);
        }

        [HttpPost("findGame")]
        public IActionResult FindGame() {
            return Unauthorized(StatusCodes.Status503ServiceUnavailable);
        }

        [HttpDelete]
        public IActionResult EndGame() {
            return Ok();
        }

        [HttpPost("postMove")]
        public IActionResult PostMove([FromBody] MoveContext move)
        {
            return Ok((new GameContext("TEST", [], "admin", 0, false), false));
        }
    }
}