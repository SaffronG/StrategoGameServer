namespace StrategoGameServer.Records
{
    public record UserWithToken(string Username, string Password, string? Token, Bot? Bot)
    {
        public UserWithToken(Account user, Guid tok) : this(user.Username, user.Password, tok.ToString(), null) { }
    }
    public record Account(string Username, string Password, string Email);
    public record OkToken(string Message, Guid Token);

    public class Game
    {
        public string User_a { get; set; }
        public string? User_b { get; set; }
        public Piece[] Board { get; set; }
        public List<MoveContext>? Moves { get; set; }

        public Game(string user_a, string? user_b, Piece[] board, List<MoveContext>? moves)
        {
            User_a = user_a;
            User_b = user_b;
            Board = board;
            Moves = moves;
        }
    }

    public record Piece(int Rank, string User, Boolean Visible);
    public record LogoutUser(string Username);
    public class Bot(int level)
    {
        public readonly int Level = level;
    }
    public record MoveContext(int LobbyId, string User, int Index_last, int Index, DateTime? Time);
    public record GameContext(string LobbyId, Piece[] Board, string User_a, string User_b, int Turn, bool IsWin);
    public record BoardRequest(int LobbyID, string Username, int Turn);
}