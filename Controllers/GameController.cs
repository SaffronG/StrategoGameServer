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
        internal static Stack<int> DefaultList()
        {
            return new(
            [
                -2,                     // FLAG 
                -1, -1, -1, -1, -1, -1, // BOMBS
                0,                      // SPY 
                9, 9, 9, 9, 9, 9, 9, 9, // SCOUTS 
                8, 8, 8, 8, 8,          // MINERS
                7, 7, 7, 7,             // SERGEANTS
                6, 6, 6, 6,             // LIEUTENANTS
                5, 5, 5, 5,             // CAPTAINS
                4, 4, 4,                // MAJORS
                3, 3,                   // COLONELS
                2,                      // GENERAL
                1                       // MARSHAL
            ]);
        }

        internal static int[] RandList(Stack<int> defaultList)
        {
            int[] tmp = new int[40];
            int count = 0;
            for (int i = 0; i < 40; i++) tmp[i] = -3;
            while (count < tmp.Length)
            {
                int rand = Random.Shared.Next(0, tmp.Length);
                if (tmp[rand] == -3)
                {
                    tmp[rand] = defaultList.Pop();
                    count++;
                }
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
                    board[i] = new(User_a[User_a_count++], "user_a", false);
                }
                else if (i > 59)
                {
                    board[i] = new(User_b[User_b_count++], "user_b", true);
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
            if (Games.Count == 0)
            {
                return NotFound("No games found.");
            }
            return Ok(Games);
        }

        [HttpPost("findGame")]
        public IActionResult FindGame([FromBody] LogoutUser user)
        {
            // Check if the user is already in a game
            for (int i = 0; i < Games.Count; i++)
            {
                if (Games[i].User_a == user.Username || Games[i].User_b == user.Username)
                {
                    // Return the current game information
                    var board = Games[i].Board;
                    if (user.Username == Games[i].User_a)
                    {
                        var reversedBoard = new List<Piece>(board);
                        reversedBoard.Reverse();
                        return Ok(new GameContext(i.ToString(), reversedBoard.ToArray(), user.Username, Games[i].Moves!.Count, false));
                    }
                    for (int j = 0; j < 40; j++) {
                        board[j] = board[j] with { Visible = false };
                    }
                    for (int j = 60; j < 100; j++) {
                        board[j] = board[j] with { Visible = true };
                    }
                    return Ok(new GameContext(i.ToString(), board, user.Username, Games[i].Moves!.Count, false));
                }
            }

            // Find an open game
            for (int i = 0; i < Games.Count; i++)
            {
                if (Games[i].User_b is null)
                {
                    Games[i] = Games[i] with { User_b = user.Username };
                    for (int j = 0; j < 40; j++) {
                        Games[i].Board[j] = Games[i].Board[j] with { Visible = false };
                    }
                    for (int j = 60; j < 100; j++) {
                        Games[i].Board[j] = Games[i].Board[j] with { Visible = true };
                    }       
                    return Ok(new GameContext(i.ToString(), Games[i].Board, user.Username, Games[i].Moves!.Count, false));
                }
            }

            // Create a new game if no open game is found
            var newGame = new Game(user.Username, null, InitBoard(), []);
            Games.Add(newGame);
            for (int j = 0; j < 40; j++) {
                newGame.Board[j] = newGame.Board[j] with { Visible = false };
            }
            for (int j = 60; j < 100; j++) {
                newGame.Board[j] = newGame.Board[j] with { Visible = true };
            }
            int newLobbyID = Games.Count - 1;
            return Ok(new GameContext(newLobbyID.ToString(), newGame.Board, user.Username, 0, false));
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
                if (game.Moves.Count > 0 && game.Moves.Last().User == move.User)
                {
                    return BadRequest("It's not your turn!");
                }
                else if (move.User == game.User_a)
                    {
                        // REVERSE BOARD
                        game.Board[99 - move.Index_last] = game.Board[99 - move.Index];
                        game.Board[99 - move.Index_last] = null!;
                    }
                    else
                    {
                        game.Board[move.Index_last] = game.Board[move.Index];
                        game.Board[move.Index] = null!;
                    }
                     if (game.Moves.Count == 0) {

                }
                else
                {
                    if (move.User == game.User_a)
                    {
                        // REVERSE BOARD
                        game.Board[99 - move.Index_last] = game.Board[99 - move.Index];
                        game.Board[99 - move.Index_last] = null!;
                    }
                    else
                    {
                        game.Board[move.Index_last] = game.Board[move.Index];
                        game.Board[move.Index] = null!;
                    }
                    game.Moves.Add(move with { Time = DateTime.Now });
                }
            }
            return Ok(game.Board);
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
            // IF GAME FOUND
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
        public IActionResult ResetBoard()
        {
            try
            {
                Games = [
                    new ("ProGamer69", "Strat3g1st", InitBoard(), []),
                ];
            }
            catch
            {
                return BadRequest("Failed to reset games!");
            }
            return Ok(Games);
        }
    }
}