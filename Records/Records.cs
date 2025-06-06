namespace StrategoGameServer.Records
{
    public record UserWithToken(string Username, string Password, string? Token, Bot? Bot)
    {
        public UserWithToken(Account user, Guid tok) : this(user.Username, user.Password, tok.ToString(), null) { }
    }
    public record Account(string Username, string Password, string Email);
    public record OkToken(string Message, Guid Token);
    public record Game(string User_a, string? User_b, Piece[] Board, List<MoveContext>? Moves, bool IsWin);
    public record Piece(int Rank, string User, Boolean Visible);
    public record LogoutUser(string Username);
    public class Bot(int level)
    {
        public readonly int Level = level;
    }
    public record MoveContext(int LobbyId, string User, int Index_last, int Index, DateTime? Time);
    public record GameContext(string LobbyId, Piece[] Board, string User_a, string User_b, int Turn, bool IsWin);
    public record BoardRequest(int LobbyID, string Username, int Turn);
    public record WinResponse(bool isWin, string message);
    public record GameRequest(string Username, Piece[]? Board)
    {
        public GameRequest(string Username) : this(Username, null) { }
    }
}