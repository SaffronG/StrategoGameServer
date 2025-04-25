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
        ];
        internal static Piece[] InitBoard(string user_a, string user_b)
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
                    board[i] = new(User_a[User_a_count++], user_a, false);
                }
                else if (i > 59)
                {
                    board[i] = new(User_b[User_b_count++], user_b, true);
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
                        for (int j = 0; j < 40; j++)
                        {
                            if (board[j] != null) board[j] = board[j] with { Visible = false };
                        }
                        for (int j = 60; j < 100; j++)
                        {
                            if (board[j] != null) board[j] = board[j] with { Visible = true };
                        }
                        return Ok(new GameContext(i.ToString(), [.. reversedBoard], user.Username, Games[i].User_b!, Games[i].Moves!.Count, false));
                    }
                    for (int j = 0; j < 40; j++)
                    {
                        if (board[j] != null) board[j] = board[j] with { Visible = false };
                    }
                    for (int j = 60; j < 100; j++)
                    {
                        if (board[j] != null) board[j] = board[j] with { Visible = true };
                    }
                    return Ok(new GameContext(i.ToString(), board, Games[i].User_a, user.Username, Games[i].Moves!.Count, false));
                }
            }

            // Find an open game
            for (int i = 0; i < Games.Count; i++)
            {
                if (Games[i].User_b is null)
                {
                    Games[i] = Games[i] with { User_b = user.Username };
                    for (int j = 0; j < Games[i].Board.Length; j++)
                    {
                        if (Games[i].Board[j] != null && Games[i].Board[j].User == "NONE")
                        {
                            Games[i].Board[j] = Games[i].Board[j] with { User = user.Username };
                        }
                    }
                    for (int j = 0; j < 40; j++)
                    {
                        Games[i].Board[j] = Games[i].Board[j] with { Visible = false };
                    }
                    for (int j = 60; j < 100; j++)
                    {
                        Games[i].Board[j] = Games[i].Board[j] with { Visible = true };
                    }
                    return Ok(new GameContext(i.ToString(), Games[i].Board, Games[i].User_a, user.Username, Games[i].Moves!.Count, false));
                }
            }

            // Create a new game if no open game is found
            var newGame = new Game(user.Username, null, InitBoard(user.Username, "NONE"), []);
            Games.Add(newGame);
            for (int j = 0; j < 40; j++)
            {
                newGame.Board[j] = newGame.Board[j] with { Visible = false };
            }
            for (int j = 60; j < 100; j++)
            {
                newGame.Board[j] = newGame.Board[j] with { Visible = true };
            }
            int newLobbyID = Games.Count - 1;
            return Ok(new GameContext(newLobbyID.ToString(), newGame.Board, user.Username, null, 0, false));
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
            Game? game = Games.FirstOrDefault(g => g.User_a == move.User || g.User_b == move.User);
            if (game == null)
            {
                return Unauthorized("Invalid Lobby ID or User");
            }

            if (game.Moves is null)
            {
                game.Moves!.Clear();
                game.Moves.Add(move with { Time = DateTime.Now });
            }
            else
            {
                if (game.Moves.Count > 0 && game.Moves.Last().User == move.User)
                {
                    return BadRequest("It's not your turn!");
                }

                else if (move.User == game.User_a)
                {
                    // Reverse board for User_a
                    List<Piece> reversedBoard = [.. game.Board];
                    reversedBoard.Reverse();
                    game.Board[move.Index] = reversedBoard[move.Index_last];
                    reversedBoard[move.Index_last] = null!;
                    reversedBoard.Reverse();
                    for (int i = 0; i < game.Board.Length; i++)
                    {
                        game.Board[i] = reversedBoard[i];
                    }
                }
                else
                {
                    game.Board[move.Index_last] = game.Board[move.Index];
                    game.Board[move.Index] = null!;
                }

                game.Moves.Add(move with { Time = DateTime.Now });
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
    }
}