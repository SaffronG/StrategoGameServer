namespace StrategoGameServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using StrategoGameServer.Records;

    [ApiController]
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
            return Ok();
        }

        [HttpPost("postMove")]
        public IActionResult PostMove([FromBody] MoveContext move)
        {
            return Ok((new GameContext("TEST", [], "admin", 0, false), false));
        }
    }
}