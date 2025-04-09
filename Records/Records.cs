namespace StrategoGameServer.Records
{
    public record UserWithToken(string Username, string Password, string? Token, Bot? Bot)
    {
        public UserWithToken(Account user, Guid tok) : this(user.Username, user.Password, tok.ToString(), null) { }
    }
    public record Account(string Username, string Password, string Email);
    public record OkToken(string Message, Guid Token);
    public record Game(UserWithToken User_a, UserWithToken? User_b, int[] Board, List<MoveContext>? Moves);
    public record LogoutUser(string Username);
    public class Bot(int level)
    {
        public readonly int Level = level;
    }
    public record MoveContext(int LobbyId, int Row, string Column, DateTime Time);
    public record GameContext(string LobbyId, int[] Board, string User, int Turn, bool IsWin);
}