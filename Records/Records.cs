namespace StrategoGameServer.Records
{
    public record UserWithToken(string Username, string Password, string? Token, Bot? Bot)
    {
        public UserWithToken(Account user, Guid tok) : this(user.Username, user.Password, tok.ToString(), null) { }
    }
    public record Account(string Username, string Password, string Email);
    public record OkToken(string Message, Guid Token);
    public record Game(string User_a, string? User_b, Piece[] Board, List<MoveContext>? Moves);
    public record Piece(int Rank, string Color);
    public record LogoutUser(string Username);
    public class Bot(int level)
    {
        public readonly int Level = level;
    }
    public record MoveContext(int LobbyId, int Row, string Column, DateTime? Time);
    public record GameContext(string LobbyId, Piece[] Board, string User, int Turn, bool IsWin);
    public record BoardRequest(int LobbyID, string Username, int Turn);
}