namespace StrategoGameServer.Controllers
{
    using System.Diagnostics;
    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.ObjectPool;
    using StrategoGameServer.Records;

    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class GameController : ControllerBase
    {
        internal static Stack<int> DefaultList() {
            return new(
            [
                -2, // FLAG 
                -1, -1, -1, -1, -1, -1, // BOMBS
                0, // SPY
                9, 9, 9, 9, 9, 9, 9, 9, // SCOUTS 
                8, 8, 8, 8, 8, // MINERS
                7, 7, 7, 7, // SERGEANTS
                6, 6, 6, 6, // LIEUTENANTS
                5, 5, 5, 5, // CAPTAINS
                4, 4, 4, // MAJORS
                3, 3, // COLONELS
                2, // GENERAL
                1 // MARSHAL
            ]);
        }

        internal static int[] RandList(Stack<int> defaultList) {
            int[] tmp = new int[40];
            for (int i = 0; i < 40; i++) tmp[i] = -3;
            while (tmp.Length < defaultList.Count) {
                int rand = Random.Shared.Next(0, tmp.Length);
                if (tmp[rand] == -3) tmp[rand] = defaultList.Pop();
            }
            return tmp;
        }
        internal static List<Game> Games =
        [
            new ("ProGamer69", "Strat3g1st", InitBoard(), []),
        ];
        internal static Piece[] InitBoard()
        {
            int User_a_count = 0;
            int User_b_count = 0;
            var User_a = RandList(DefaultList());
            var User_b = RandList(DefaultList());
            var board = new Piece[100];
            Random r = new();
            for (int i = 0; i < 100; i++)
            {
                if (i < 40)
                {
                    board[i] = new(User_a[User_a_count++], "user_a");
                }
                else if (i > 59)
                {
                    board[i] = new(User_b[User_b_count++], "user_b");
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
            return Ok(Games);
        }

        [HttpPost("findGame")]
        public IActionResult FindGame([FromBody] LogoutUser user)
        {
            Game? openGame = null;
            int LobbyID = 0;
            for (int i = 0; i < Games.Count; i++)
            {
                // FIND OPEN GAME
                if (Games[i].User_b is null || Games[i].User_a is null)
                {
                    openGame = Games[i];
                    LobbyID = i;
                    break;
                }
                 if (Games[i].User_a == user.Username || Games[i].User_b == user.Username) {
                    return Ok(new GameContext(i.ToString(), Games[i].Board, user.Username, Games[i].Moves!.Count, false));
                }
            }
            if (openGame == null)
            {
                // CREATE A NEW GAME
                Games[LobbyID] = new(user.Username, null, InitBoard(), []);
                Games.Add(openGame!);
                LobbyID = Games.Count - 1;
                return Ok(new GameContext(LobbyID.ToString(), Games[LobbyID].Board, user.Username, 0, false));

            }
            else if (openGame.User_b == null)
            {
                // JOIN EXISTING GAME IF OPEN GAME IS FOUND
                Games[LobbyID] = openGame with { User_b = user.Username };
                return Ok(new GameContext(LobbyID.ToString(), openGame.Board, user.Username, 0, false));
            }
            else
            {
                return Unauthorized("No available games found.");
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
                return Unauthorized("Invalid Lobby ID");
            }
            if (currentGame.Moves!.Count <= request.Turn)
            {
                return Ok("Waiting for opponent...");
            }
            else
            {
                return Ok(currentGame.Board);
            }
        }

        [HttpGet("ResetBoard")]
        public IActionResult ResetBoard() {
            try {
                Games = [
                    new ("ProGamer69", "Strat3g1st", InitBoard(), []),
                ];
            } catch {
                return BadRequest("Failed to reset games!");
            }
            return Ok(Games);
        }
    }
}