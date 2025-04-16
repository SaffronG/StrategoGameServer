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
        private static readonly List<Game> Games = [
            new ("test", "test", [], []),
            new ("test_2", "test_2", [], []),
            new ("test_3", "test_3", [], []),
            new ("test_4", "test_4", [], []),
            new ("test_5", "test_5", [], []),
        ];

        [HttpGet("getGames")]
        public IActionResult GetGames()
        {
            if (Games.Count < 1)
            {
                return Ok("No Games Found");
            }
            return Ok(Games);
        }

        [HttpGet("findGame")]
        public IActionResult FindGame([FromQuery] string username)
        {
            Game? openGame = Games.FirstOrDefault(g => g.User_b is null) ?? null;
            if (Games.Count < 1 && openGame == null)
            {
                Console.WriteLine(openGame);
                openGame = new(username, null, new Piece[100], []);
                Games.Add(openGame);
                return Ok(openGame);
            }
            else
            {
                Console.WriteLine(openGame);
                if (openGame != null)
                    return Ok(openGame with { User_b = username });
            }
            Console.WriteLine(openGame);
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
            else
            {
                game.Moves.Add(move with { Time = DateTime.Now });
            }
            return Ok(game);
        }
        [HttpGet("getBoard")]
        public IActionResult GetBoard([FromBody] BoardRequest request)
        {
            Piece[] board;
            try
            {
                board = Games[request.LobbyID].Board;
            }
            catch
            {
                return Forbid();
            }
            var tmp = new Piece[100];
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i].Color != request.Color)
                {
                    tmp[i] = new(0, request.Color == "red" ? "blue" : "red");
                }
                else
                {
                    tmp[i] = board[i];
                }
            }
            return Ok(tmp);
        }
    }
}