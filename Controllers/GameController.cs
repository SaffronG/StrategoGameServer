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
        internal static List<Game> Games =
        [
            new ("ProGamer69", "Strat3g1st", InitBoard(), []),
            new ("test", null, InitBoard(), []),
        ];

        internal static Piece[] InitBoard()
        {
            var board = new Piece[100];
            Random r = new();
            for (int i = 0; i < 100; i++)
            {
                if (i < 40)
                {
                    board[i] = new(r.Next(-1, 11), "red");
                }
                else if (i > 59)
                {
                    board[i] = new(r.Next(-1, 11), "blue");
                }
                else
                {
                    board[i] = null!;
                }
            }
            return board;
        }

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
        public IActionResult FindGame([FromBody] LogoutUser user)
        {
            Game? openGame = null;
            int LobbyID = 0;
            for (int i = 0; i < Games.Count; i++)
            {
                if (Games[i].User_b is null)
                {
                    openGame = Games[i];
                    LobbyID = i;
                    break;
                }
            }
            if (openGame == null)
            {
                Games[LobbyID] = new(user.Username, null, new Piece[100], []);
                Games.Add(openGame!);
                return Ok(new GameContext(LobbyID.ToString(), Games[LobbyID].Board, user.Username, 0, false));

            }

            else if (openGame.User_b == null)
            {
                Games[LobbyID] = openGame with { User_b = user.Username };
                return Ok(new GameContext(LobbyID.ToString(), openGame.Board, user.Username, 0, false));
            }
            else
            {
                return BadRequest("No available games found.");
            }
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
        [HttpPost("getBoard")]
        public IActionResult GetBoard([FromBody] BoardRequest request)
        {
            Game currentGame;
            try
            {
                currentGame = Games[request.LobbyID];
            }
            catch
            {
                return Unauthorized();
            }
            if (currentGame.Moves!.Count < request.turn)
            {
                return Ok("Waiting for opponent...");
            }
            else
            {
                return Ok(currentGame.Board);
            }
        }
    }
}