namespace StrategoGameServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using StrategoGameServer.Records;


    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AuthController : ControllerBase
    {
        private static readonly List<Account> Users =
        [
            new("admin", "password", "admin.123@fake.com"),
            new("user", "1234", "guest@fake.com"),
            new("guest", "password", "guest123@fake.com"),
        ];

        private static readonly List<UserWithToken> Authenticated = [];

        [HttpPost("login")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public IActionResult Login([FromBody] Account user)
        {
            var foundUser = Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (foundUser == null) return Unauthorized();

            var isLoggedIn = Authenticated.FirstOrDefault(u => u.Username == foundUser.Username);
            if (isLoggedIn != null) return Conflict("User is already logged in!");

            var token = Guid.NewGuid();
            Authenticated.Add(new UserWithToken(foundUser, token));
            return Ok(new OkToken("Login successful", token));
        }

        [HttpPost("register")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public IActionResult Register([FromBody] Account user)
        {
            if (Users.Any(u => u.Username == user.Username)) return Conflict("User already exists");

            Users.Add(user);
            return Created($"/user/{user.Username}", user);
        }

        [HttpPost("logout")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public IActionResult Logout([FromBody] LogoutUser logoutUser)
        {
            var user = Authenticated.FirstOrDefault(u => u.Username == logoutUser.Username);
            if (user == null) return Forbid();

            Authenticated.RemoveAll(u => u.Username == logoutUser.Username);
            return Ok("Logout successful");
        }

        [HttpGet("concurrentUsers")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IActionResult GetConcurrentUsers()
        {
            return Ok(Authenticated);
        }

        [HttpGet("getAccount")]
        public IActionResult GetAccount([FromQuery] string Username) {
            return Ok(Authenticated.FirstOrDefault(u => u.Username == Username));
        }
    }
}