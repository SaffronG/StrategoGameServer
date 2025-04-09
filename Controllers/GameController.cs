namespace StrategoGameServer.Controllers
{
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using StrategoGameServer.Records;

    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class GameController : ControllerBase
    {
        private static readonly List<Game> Games = [];

        [HttpGet("getGames")]
        public IActionResult GetGames()
        {
            if (Games.Count < 1)
            {
                return Ok("No Games Found");
            }
            return Ok(Games);
        }

        [HttpPost("findGame")]
        public IActionResult FindGame([FromBody] UserWithToken user)
        {
            Game? openGame = Games.FirstOrDefault(g => g.User_b is null) ?? null;
            if (Games.Count < 1 && openGame == null)
            {
                openGame = new(user, null, [100], []);
                Games.Add(openGame);
                return Ok(openGame);
            }
            else
            {
                if (openGame != null)
                    return Ok(openGame with { User_b = user });
            }
            return Unauthorized(StatusCodes.Status503ServiceUnavailable);
        }

        [HttpDelete("endGame")]
        public IActionResult EndGame([FromBody] int LobbyId)
        {
            if (Games.Count > LobbyId)
            {
                try { Games.RemoveAt(LobbyId); }
                catch (ArgumentOutOfRangeException) { return BadRequest($"Invalid LobbyID: {LobbyId} Not Found"); }
            }
            else
            {
                return Forbid($"Invalid LobbyID: {LobbyId} Not Found");
            }
            return Ok();
        }

        [HttpPost("postMove")]
        public IActionResult PostMove([FromBody] MoveContext move)
        {
            Game? game = Games[move.LobbyId];
            if (game.Moves is null) game = game with { Moves = [move with { Time = DateTime.Now }] };
            else {
                game.Moves.Add(move with { Time = DateTime.Now });
            }
            return Ok(game);
        }
    }
}